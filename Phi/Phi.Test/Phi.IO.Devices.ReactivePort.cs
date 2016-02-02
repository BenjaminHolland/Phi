using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phi.IO;
using Phi.IO.Devices;
using System.Threading;
using Phi.Core;
using System.Text;

namespace Phi.IO.Devices.Test {
    [TestClass]
    public class TestReactivePort {
        class PingFake:DisposableObject {
            public bool Started {
                get;
                private set;
            }
            CancellationTokenSource _cts;
            ReactivePort _port;
            Thread _loopThread;
            void _loop() {
                while (!_cts.Token.IsCancellationRequested) {
                    Thread.Sleep(1000);
                    _port.Write("Ping\n");
                }
            }
            public void Start() {
                if (!Started) {
                    _port = new ReactivePort();
                    _port.BaudRate = 115200;
                    _port.PortName = "COM7";
                    _port.Open();
                    _cts = new CancellationTokenSource();
                    _loopThread = new Thread(_loop);
                    _loopThread.Start();
                }
            }
            protected override void DisposeManagedResources() {
                _cts.Cancel();
                _loopThread.Join();
                _port.Close();
                Started = false;
                base.DisposeManagedResources();
            }

        }
        class EchoFake : DisposableObject {
            public bool Started {
                get;
                private set;
            }

            ReactivePort _port;
            IDisposable _sub;
            public void Start() {
                if (!Started) {
                    _port = new ReactivePort();
                    _port.BaudRate = 115200;
                    _port.PortName = "COM7";
                    _sub = _port.Received.Subscribe(data => _port.Write(data.Data, 0, data.Data.Length));
                    _port.Open();
                  
                }
            }
            protected override void DisposeManagedResources() {
                _sub.Dispose();
                _port.Close();
                Started = false;
                base.DisposeManagedResources();
            }

        }
        [TestMethod]
        [TestCategory("Phi.IO.Devices.ReactivePort")]
        public void OpenCloseTest() {
            string portName = "COM4";

            ReactivePort port = new ReactivePort();
            port.PortName = portName;
            port.Open();
            Assert.IsTrue(port.IsOpen);
            port.Close();

            Assert.IsFalse(port.IsOpen);
            port.Dispose();
            
        }
        [TestMethod]
        [TestCategory("Phi.IO.Devices.ReactivePort")]
        public void RepeatOpenCloseTest() {
            string portName = "COM4";

            ReactivePort port = new ReactivePort();
            port.PortName = portName;
            for (int i = 0; i < 100; i++) {
                port.Open();
                Assert.IsTrue(port.IsOpen);
                port.Close();
                Assert.IsFalse(port.IsOpen);
            }
            port.Dispose();
        }
        [TestMethod]
        [TestCategory("Phi.IO.Devices.ReactivePort")]
        public void SubscribeReceivedTest() {
            PingFake f = new PingFake();
            f.Start();
            ReactivePort p = new ReactivePort();
            p.PortName = "COM8";
            p.BaudRate = 115200;
            p.Open();
            IDisposable sub=p.Received.Subscribe(x => Console.WriteLine("<< "+Encoding.ASCII.GetString(x.Data)));
            Thread.Sleep(10000);
            sub.Dispose();
            p.Dispose();
            f.Dispose();
        }
        
        [TestMethod]
        [TestCategory("Phi.IO.Devices.ReactivePort")]
        public void SubscribeSentTest() {
            EchoFake f = new EchoFake();
            f.Start();
            ReactivePort p = new ReactivePort();
            p.PortName = "COM8";
            p.BaudRate = 115200;
            p.Open();
            var sub = p.Sent.Subscribe(x => Console.WriteLine(">> "+Encoding.ASCII.GetString(x.Data)));
            for (int i = 0; i < 10; i++) {
                p.Write("Ping\n");
            }
            sub.Dispose();
            f.Dispose();
            p.Dispose();

           
        }
    }
}
