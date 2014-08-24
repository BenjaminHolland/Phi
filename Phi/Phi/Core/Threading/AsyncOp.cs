using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Phi.Core.Threading {
    public delegate Task<RT> AsyncOpDelegate<RT,PT>(IProgress<PT> p,CancellationToken ct); 
    /// <summary>
    /// Class for controlling async operations. The operations should be cancelable and return a value.
    /// </summary>
    /// <typeparam name="RT">The return value of the operation</typeparam>
    /// <typeparam name="PT">The progress type returned by the operation</typeparam>
    [Phi.Core.Development.TODO("This might be better as an abstract base class rather than an instance itself.")]
    public class AsyncOp<RT,PT>:Phi.Core.BindableDisposableObject {
        protected ManualResetEventSlim _blockingHandle;
        protected AsyncOpDelegate<RT, PT> _asyncOp;
        protected CancellationTokenSource _cts;
        protected Progress<PT> _progress;
        private bool _isRunning;
        public bool IsRunning {
            get {
                return _isRunning;
            }
            set {
                _isRunning = value;
                onPropertyChanged("IsRunning");
            }
        }
        public AsyncOp(AsyncOpDelegate<RT, PT> asyncOp) {
            _blockingHandle = new ManualResetEventSlim();
            _asyncOp = asyncOp;
            _cts = new CancellationTokenSource();
            _progress=new Progress<PT>();
            _isRunning = false;
        }
        //Run the operation.
        public async Task<Optional<RT>> RunAsync() {
            if (IsDisposed) {
                throw new ObjectDisposedException("AsyncOp");
            }
            if (_isRunning) {
                throw new InvalidOperationException("Already running");
            }
            Optional<RT> ret;
            try {
                _blockingHandle.Reset();
                IsRunning = true;
                ret=new Optional<RT>(await _asyncOp(_progress, _cts.Token));
            }
            catch (OperationCanceledException) {
                _cts = new CancellationTokenSource();
                ret = new Optional<RT>();
            }
            finally {
                _blockingHandle.Set();
                IsRunning = false;
            }
            return ret;
        }
        /// <summary>
        /// Cancel the operation.
        /// </summary>
        public void Cancel() {
            if (IsDisposed) {
                throw new ObjectDisposedException("AsyncOp");
            }
            if (!_isRunning) {
                throw new InvalidOperationException("Not running.");
            }
            _cts.Cancel();
        }
        public void CancelBlocking() {
            if (IsDisposed) {
                throw new ObjectDisposedException("AsyncOp");
            }
            if (!_isRunning) {
                throw new InvalidOperationException("Not running.");
            }
            else {
                _cts.Cancel();
                _blockingHandle.Wait();
            }

        }
        protected override void DisposeUnmanagedResources() {
            CancelBlocking(); 
            _cts.Dispose();
            base.DisposeUnmanagedResources();
        }
        ~AsyncOp() {
            Dispose(false);
            GC.SuppressFinalize(this);
        }
    }
}
