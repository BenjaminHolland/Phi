using RJCP.IO.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Phi.IO.Ports
{
    public sealed class ReactivePort:IDisposable
    {
        private SerialPortStream _port;
        #region Received Infrastructure
        private sealed class ReceivedGeneratorState
        {
            private ReactivePort _parent;
            private CancellationTokenSource _cts;
            public ReceivedGeneratorState(ReactivePort parent)
            {
                _parent = parent;
                _cts = new CancellationTokenSource();
            }
            public int BufferWritePosition
            {
                get;
                set;
            }
            public bool Reading
            {
                get;
                set;
            }
            public bool CanRead
            {
                get
                {
                    return _parent._port.IsOpen;
                }
            }
            public bool ShutdownRequested
            {
                get
                {
                    return _cts.Token.IsCancellationRequested;
                }
            }
            public void Shutdown()
            {

                _cts.Cancel();
            }
        }
        private ReceivedGeneratorState _receivedGeneratorState;
        private IObservable<byte[]> _receivedGenerator;
        private byte[] _receivedLoopBody(ReceivedGeneratorState state)
        {
            int read = 0;

            //Try and read into the buffer untill some bytes arrive
            StringBuilder trace = new StringBuilder();
            do
            {
                string s = trace.ToString();
                //Console.WriteLine(s);
                trace.Clear();
                read = _port.Read(_receivedBuffer, state.BufferWritePosition, _receivedBuffer.Length - state.BufferWritePosition); //Waits at least ReadTimeout ms
                //trace.AppendLine($"Read {read} bytes");
                //If we're not reading, and we read nothing, try again.
                if (!state.Reading && read == 0)
                {
                    // trace.AppendLine("Not Reading-No Bytes");
                    continue;
                }
                //If we're not reading and we read bytes
                if (!state.Reading && read > 0)
                {
                    // trace.AppendLine("Not reading-New Bytes");
                    //Scan the new bytes for a newline
                    int nlPointer = -1;
                    for (int cp = state.BufferWritePosition; cp < state.BufferWritePosition + read; cp++)
                    {
                        if (_receivedBuffer[cp] == '\n')
                        {
                            nlPointer = cp;
                            break;
                        }
                    }
                    //If we have a newline, prepare for the next iteration and return data;
                    if (nlPointer != -1)
                    {
                        byte[] bytes = new byte[nlPointer + 1];
                        Buffer.BlockCopy(_receivedBuffer, 0, bytes, 0, nlPointer + 1); //Copy bytes into the return buffer.
                        Buffer.BlockCopy(_receivedBuffer, nlPointer + 1, _receivedBuffer, 0, _receivedBuffer.Length - (nlPointer + 1)); //Shift data up.
                        state.BufferWritePosition += read;
                        state.BufferWritePosition -= nlPointer + 1;
                        //If we recieved exactly enough for the newline, restart.
                        if (state.BufferWritePosition == 0)
                        {
                            state.Reading = false;
                        }
                        //trace.AppendLine("Newline Return");
                        //Console.WriteLine(trace.ToString());
                        return bytes;
                    }
                    //Otherwise, buffer the new data and try to get more
                    else
                    {
                        trace.AppendLine("No Newline Continue");
                        state.BufferWritePosition += read;
                        state.Reading = true;
                        continue;
                    }
                }
                //If we're already reading and read nothing, return the whole buffer and reset the write pointer.
                if (state.Reading && (read == 0))
                {

                    //trace.AppendLine("Reading-No Bytes");
                    byte[] bytes = new byte[state.BufferWritePosition];
                    Buffer.BlockCopy(_receivedBuffer, 0, bytes, 0, state.BufferWritePosition);
                    state.BufferWritePosition = 0;
                    state.Reading = false;

                    //trace.AppendLine("Timeout Return");
                    //Console.WriteLine(trace.ToString());
                    return new bytes;
                }
                //If we're reading and we get bytes. 
                if (state.Reading && (read != 0))
                {

                    //trace.AppendLine("Reading-New Bytes");
                    //Scan the new bytes for a newline
                    int nlPointer = -1;
                    for (int cp = state.BufferWritePosition; cp < state.BufferWritePosition + read; cp++)
                    {
                        if (_receivedBuffer[cp] == '\n')
                        {
                            nlPointer = cp;
                            break;
                        }
                    }
                    //If we have a newline, prepare for the next iteration and return data;
                    if (nlPointer != -1)
                    {
                        byte[] bytes = new byte[nlPointer + 1];
                        Buffer.BlockCopy(_receivedBuffer, 0, bytes, 0, nlPointer + 1); //Copy bytes into the return buffer.
                        Buffer.BlockCopy(_receivedBuffer, nlPointer + 1, _receivedBuffer, 0, _receivedBuffer.Length - (nlPointer + 1)); //Shift data up.
                        state.BufferWritePosition += read;
                        state.BufferWritePosition -= nlPointer + 1;
                        if (state.BufferWritePosition == 0)
                        {
                            state.Reading = false;
                        }
                        //trace.AppendLine("Reading-Newline Return");
                        return bytes;
                    }
                    //Otherwise, buffer the new data and try to get more
                    else
                    {
                        state.BufferWritePosition += read;
                        //trace.AppendLine("Reading-Continue");
                        continue;
                    }
                }

            } while ((state.Reading ? (read > 0) : (read == 0)) && !state.ShutdownRequested && state.CanRead);
            //Console.WriteLine("Weird Ending");
            return new byte[0];
        }
        private readonly byte[] _receivedBuffer = new byte[2048];
        private Subject<byte[]> _receivedSubject;
        private IDisposable _receivedSub;
        #endregion

        #region Sent Infrastructure
        private Subject<byte[]> _sentSubject;
        #endregion

        #region Lifetime
        private bool _isDisposed;
        public ReactivePort()
        {
            //Create the underlying serial connection.
            _port = new SerialPortStream();
            //Create a subject that will manage the distribution of the received messages.
            _receivedSubject = new Subject<byte[]>();

            //Create the state the received generator will use.
            _receivedGeneratorState = new ReceivedGeneratorState(this);

            //Create a message generator that polls the serial port for data and sends a new message either when a read times out, or when a \n is detected.
            //Make sure this generator runs on a different thread so we don't block. There should be a way to make this async, but I'm not sure how.
            _receivedGenerator = Observable
                .Generate(
                    _receivedGeneratorState,
                    state => !state.ShutdownRequested,
                    state => state,
                    _receivedLoopBody,
                    NewThreadScheduler.Default);
            //Create a subject that will dispatch data written to the port.
            _sentSubject = new Subject<byte[]>();
        }
        public bool IsDisposed
        {
            get;
            private set;
        }
        private void Dispose(bool disposing)
        {
            if (IsDisposed) return;
            if (disposing)
            {
                Close();
                _receivedSubject.OnCompleted();
                _sentSubject.OnCompleted();
                _receivedGeneratorState.Shutdown();
                SpinWait.SpinUntil(_receivedGenerator.IsEmpty().Wait);
                _port.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void ThrowIfDisposed()
        {
            if (IsDisposed) throw new ObjectDisposedException("this");
        }
        #region Output
        public IObservable<byte[]> Sent
        {
            get
            {
                return _sentSubject.AsObservable();
            }
        }
        public int WriteTimeout
        {
            get
            {
                return _port.WriteTimeout;
            }
            set
            {
                _port.WriteTimeout = value;
            }
        }
        public bool RtsEnable
        {
            get
            {
                return _port.RtsEnable;
            }
            set
            {
                _port.RtsEnable = value;
            }
        }
        public void Flush()
        {
            _port.Flush();
        }
        public void DiscardOutBuffer()
        {
            _port.DiscardOutBuffer();
        }
       
        public void Write(byte[] buffer, int offset, int count)
        {
            if (!_port.IsOpen) return;
            _port.Write(buffer, offset, count);
            byte[] tx = new byte[count];
            Buffer.BlockCopy(buffer, offset, tx, 0, count);
            _sentSubject.OnNext(tx);
        }
        public void Write(string data)
        {
            if (!_port.IsOpen) return;
            _port.Write(data);
            _sentSubject.OnNext(_port.Encoding.GetBytes(data));
        }
        public void Write(string fmt, params string[] args)
        {
            Write(string.Format(fmt, args));
        }
        #endregion

        #region Input
        public IObservable<byte[]> Received
        {
            get
            {
                return _receivedSubject.AsObservable();
            }
        }
        public int ReadTimeout
        {
            get
            {
                return _port.ReadTimeout;
            }
            set
            {
                _port.ReadTimeout = value;
            }
        }
        public bool DtrEnable
        {
            get
            {
                return _port.DtrEnable;
            }
            set
            {
                _port.DtrEnable = value;
            }
        }
        public void DiscardInBuffer()
        {
            _port.DiscardInBuffer();
        }
        #endregion

        public bool BreakState
        {
            get
            {

                return _port.BreakState;
            }
            set
            {
                _port.BreakState = value;
            }
        }

        public int BaudRate
        {
            get
            {

                return _port.BaudRate;
            }
            set
            {
                _port.BaudRate = value;
            }
        }
        public string PortName
        {
            get
            {

                return _port.PortName;
            }
            set
            {
                _port.PortName = value;
            }
        }
        public bool IsOpen
        {
            get
            {
                ThrowIfDisposed();
                return _port.IsOpen;
            }
        }
        /// <summary>
        /// Opens the port.
        /// </summary>
        /// <remarks>
        /// Internally, all this does is attach the message generator to the Received subject and open the underlying port.
        /// it does not stop the received generator.
        /// </remarks>

        public void Open()
        {
            ThrowIfDisposed();
            if (_port.IsOpen) return;
            _port.Open();
            _receivedSub = _receivedGenerator.Subscribe(_receivedSubject);
        }
        /// <summary>
        /// Closes the port.
        /// </summary>
        /// <remarks>
        /// Internally, simply disconnects the received subject from the received generator.</remarks>
        public void Close()
        {
            ThrowIfDisposed();
            if (!_port.IsOpen) return;
            _receivedSub.Dispose();
            _port.Close();
        }

  
    }
}
