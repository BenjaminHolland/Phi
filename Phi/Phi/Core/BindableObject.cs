using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace Phi.Core {
    public abstract class BindableObject : INotifyPropertyChanged {
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
    }
}
