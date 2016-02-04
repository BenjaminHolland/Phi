using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phi.Core;
using System.Windows.Input;

namespace Phi.IO.Devices {
    public class ReactivePortViewModel : ViewModelBase, IDisposableObject {
        private ReactivePort _port;
        public ReactivePortViewModel() {
            _port = new ReactivePort();
        }

        public string PortName {
            get {
                return _port.PortName;
            }
            set {
                string storage = _port.PortName;
                if (SetProperty(ref storage, value, "PortName")) {
                    _port.PortName = storage;
                }

            }
        }
        public int BaudRate {
            get {
                return _port.BaudRate;
            }
            set {
                int storage = _port.BaudRate;
                if(SetProperty(ref storage, value, "BaudRate")) {
                    _port.BaudRate = storage;
                }
            }
        }

        public bool IsOpen {
            get {
                return _port.IsOpen;
            }
        }
        
        public void Open() {
            bool storage = _port.IsOpen;
            _port.Open();
            SetProperty(ref storage, _port.IsOpen, "IsOpen");
        }
        public void Close() {
            bool storage = _port.IsOpen;
            _port.Close();
            SetProperty(ref storage, _port.IsOpen, "IsOpen");
        }
        public bool IsDisposed {
            get {
                return _port.IsDisposed;
            }
        }

        public IObservable<ReactivePortPacket> Received {
            get {
                return _port.Received;
            }
        }
        public IObservable<ReactivePortPacket> Sent {
            get {
                return _port.Sent;
            }
        }
        public event EventHandler<DisposingEventArgs> Disposing {
            add {
                _port.Disposing += value;
            }
            remove {
                _port.Disposing -= value;
            }
        }
        public event EventHandler<DisposedEventArgs> Disposed {
            add {
                _port.Disposed += value;
            }
            remove {
                _port.Disposed -= value;
            }
        }

        public void Dispose() {
            _port.Dispose();
        }
    }
}
