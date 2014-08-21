using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
namespace Phi.Core.Threading {
    public interface IAsyncObjectWaitResult<T> {
        bool HasValue {
            get;
        }
        T Value {
            get;
        }
    }
    public class AsyncObjectWait<TriggerType>:DisposableObject {
        private class Result : IAsyncObjectWaitResult<TriggerType> {
            public Result(bool hasValue, TriggerType value) {
                HasValue = hasValue;
                Value = value;
            }
            public bool HasValue {
                get;
                private set;
            }

            public TriggerType Value {
                get;
                private set;
            }
        }
        private BlockingCollection<TriggerType> _item;
        Predicate<TriggerType> _shouldTrigger;
        /// <summary>
        /// Create a new AsyncWaitObject that will trigger when a trigger value, when converted with converter, equals the triggerValue
        /// </summary>
        /// <param name="triggerValue">The value to check against</param>
        /// <param name="converter">a function that will convert the trigger type into a trigger value type</param>
        public AsyncObjectWait(Predicate<TriggerType> shouldTrigger) {
            _shouldTrigger = shouldTrigger;
            _item = new BlockingCollection<TriggerType>(new ConcurrentQueue<TriggerType>(), 1);
        }
        /// <summary>
        /// Wait Synchronously for an object to trigger the handle
        /// </summary>
        /// <param name="timeout">How long to wait for the handle to trigger.</param>
        /// <param name="triggeringObj">The object that triggerd the handle if the handle was triggered, otherwise default(TriggerType)</param>
        /// <returns>true if the handle was triggered, false otherwise</returns>
        public bool WaitOne(TimeSpan timeout, out TriggerType triggeringObj) {
            return _item.TryTake(out triggeringObj, timeout);
        }
        /// <summary>
        /// Wait Asynchronously for an object to trigger the handle
        /// <returns>A task that can be waited on to get the triggering object</returns>
        public Task<IAsyncObjectWaitResult<TriggerType>> WaitOneAsync(CancellationToken ct) {
            return Task<IAsyncObjectWaitResult<TriggerType>>.Run(() => {
                Result res;
                TriggerType triggeringObj = default(TriggerType);
                _item.TryTake(out triggeringObj, Timeout.Infinite,ct);
                if (_item.TryTake(out triggeringObj, Timeout.Infinite, ct)) {
                    res = new Result(true, triggeringObj);
                }
                else {
                    res = new Result(false, triggeringObj);
                }
                return (IAsyncObjectWaitResult<TriggerType>)res;
            },ct);
        }
        /// <summary>
        /// Wait Asynchronously for an object to trigger the handle
        /// </summary>
        /// <param name="timeout">How long unitl this call times out</param>
        /// <returns>A task that can be awaited to get the triggering object. If the task times out, the triggering object is default(TriggerType)</returns>
        public Task<IAsyncObjectWaitResult<TriggerType>> WaitOneAsync(TimeSpan timeout,CancellationToken ct) {
            return Task<IAsyncObjectWaitResult<TriggerType>>.Run(() => {
                Result res;
                TriggerType triggeringObj = default(TriggerType);
                if (_item.TryTake(out triggeringObj, (int)timeout.TotalMilliseconds, ct)) {
                    res = new Result(true, triggeringObj);
                }
                else {
                    res = new Result(false, triggeringObj);
                }
                return (IAsyncObjectWaitResult<TriggerType>)res;
            },ct);

        }
        /// <summary>
        /// Attempt to trigger the wait object. Only triggers that, when converted, match the triggerValue will trigger the handle.
        /// </summary>
        /// <param name="trigger">The object to attempt to trigger the handle with</param>
        /// <returns>true if the object was able to actually trigger the handle, false otherwise</returns>
        public bool TrySetOne(TriggerType trigger) {
            if (_shouldTrigger(trigger)) {
                return _item.TryAdd(trigger);
            }
            else {
                return false;
            }
        }
        protected override void DisposeManagedResources() {
            _item.Dispose();
            base.DisposeManagedResources();
        }
        ~AsyncObjectWait() {
            Dispose(false);
        }
    }
}
