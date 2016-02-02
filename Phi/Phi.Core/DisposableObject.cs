using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.Core {
    public class DisposedEventArgs : EventArgs {
        public static readonly DisposedEventArgs Current=new DisposedEventArgs();
        public DisposedEventArgs()
            : base() {
        }
    }
    public class DisposingEventArgs : EventArgs {
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
        public event EventHandler<DisposingEventArgs> Disposing;
        public event EventHandler<DisposedEventArgs> Disposed;
        protected virtual void OnDisposed() {
            if (Disposed != null) {
                Disposed(null, DisposedEventArgs.Current);
            }
        }
        protected virtual void OnDisposing() {
            if (Disposing != null) {
                Disposing(this, DisposingEventArgs.Current);
            }
        }
        protected virtual void Dispose(bool disposing) {
            if (!_isDisposed) {
                OnDisposing();
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
