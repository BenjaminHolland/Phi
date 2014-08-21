using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace Phi.Core {
    public class BindableDisposableObject : IDisposableObject, INotifyPropertyChanged {
        private static Dictionary<string, PropertyChangedEventArgs> propertyChangedEventArgsCache = new Dictionary<string, PropertyChangedEventArgs>();
        protected virtual void onPropertyChanged(string property) {
            if (PropertyChanged != null) {
                PropertyChanged(this, getPropertyChangedEventArgs(property));
            }
        }
        protected PropertyChangedEventArgs getPropertyChangedEventArgs(string property) {
            PropertyChangedEventArgs ret;
            lock (typeof(BindableObject)) {
                if (!propertyChangedEventArgsCache.ContainsKey(property)) {
                    ret = new PropertyChangedEventArgs(property);
                    propertyChangedEventArgsCache[property] = ret;
                }
                else {
                    ret = propertyChangedEventArgsCache[property];
                }
            }
            return ret;
        }
        public event PropertyChangedEventHandler PropertyChanged;
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
