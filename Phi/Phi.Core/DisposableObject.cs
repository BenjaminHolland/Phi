using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Core {
    public sealed class DisposedEventArgs : EventArgs {
        public static readonly DisposedEventArgs Current=new DisposedEventArgs();
        public DisposedEventArgs()
            : base() {
        }
    }
    public sealed class DisposingEventArgs : EventArgs {
        public static readonly DisposingEventArgs Current = new DisposingEventArgs();
        public DisposingEventArgs() :
            base() {
        }
    }
    public interface IDisposableObject : IDisposable {
        bool IsDisposed {
            get;
        }
        event EventHandler<DisposingEventArgs> Disposing;
        event EventHandler<DisposedEventArgs> Disposed;
    }
    public class DisposableObject : IDisposableObject {
        private bool _isDisposed;
        
        public virtual bool IsDisposed {
            get {
                return _isDisposed;
            }
            protected set {
                _isDisposed = value;
            }
        }
        public event EventHandler<DisposingEventArgs> Disposing;
        public event EventHandler<DisposedEventArgs> Disposed;
        protected void ThrowIfDisposed() {
            if (_isDisposed) throw new ObjectDisposedException(ToString());
        }
        protected virtual void RaiseDisposed() {
            if (Disposed != null) {
                Disposed(null, DisposedEventArgs.Current);
            }
        }
        protected virtual void RaiseDisposing() {
            if (Disposing != null) {
                Disposing(this, DisposingEventArgs.Current);
            }
        }
        protected virtual void Dispose(bool disposing) {
            if (!_isDisposed) {
                RaiseDisposing();
                if (disposing) {
                    DisposeManagedResources();
                }
                DisposeUnmanagedResources();
                IsDisposed = true;
                RaiseDisposed();
            }
        }
        protected virtual void DisposeManagedResources() { }
        protected virtual void DisposeUnmanagedResources() { }
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~DisposableObject() {
            Dispose(false);
        }
    }
}
