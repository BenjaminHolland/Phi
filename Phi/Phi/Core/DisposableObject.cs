using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Core {
    public class DisposedEventArgs : EventArgs {
        public DisposedEventArgs()
            : base() {
        }
    }
    public delegate void DisposedEventHandler(object sender, DisposedEventArgs e);
    public interface IDisposableObject : IDisposable {
        bool IsDisposed {
            get;
        }
        event DisposedEventHandler Disposed;
    }
    public abstract class DisposableObject : IDisposableObject {
        private bool _isDisposed;

        public virtual bool IsDisposed {
            get {
                return _isDisposed;
            }
            protected set {
                _isDisposed = value;
            }
        }
        public event DisposedEventHandler Disposed;

        protected virtual void OnDisposed() {
            if (Disposed != null) {
                Disposed(null, new DisposedEventArgs());
            }
        }
        protected virtual void Dispose(bool disposing) {
            if (!_isDisposed) {
                if (disposing) {
                    DisposeManagedResources();
                }
                DisposeUnmanagedResources();
                IsDisposed = true;
                OnDisposed();
            }
        }
        protected virtual void DisposeManagedResources() {

        }
        protected virtual void DisposeUnmanagedResources() {

        }
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
