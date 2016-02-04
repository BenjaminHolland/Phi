using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections;

namespace Phi.Core
{
    public class ErrorCollection<T> where T :IEquatable<T>
    {
        private Dictionary<string, List<T>> _errors;
        public IEnumerable<T> GetPropertyErrors(string propertyName)
        {
            EnsureEntry(propertyName);
            return _errors[propertyName];
        }
        public void ClearPropertyErrors(string propertyName)
        {
            EnsureEntry(propertyName);
            
            _errors[propertyName].Clear();
        }
        public void AddPropertyError(string propertyName,T error)
        {
            EnsureEntry(propertyName);
            _errors[propertyName].Add(error);
        }
        public int ErrorCount
        {
            get
            {
                return (from entry in _errors select entry.Value.Count).Sum();
            }
        }
        public void EnsureEntry(string propertyName)
        {
            if(!_errors.Keys.Contains(propertyName))
            {
                _errors.Add(propertyName, new List<T>());
            }
        }
        public ErrorCollection()
        {
            _errors = new Dictionary<string, List<T>>();
        }
    }
    public class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        static Dictionary<string, DataErrorsChangedEventArgs> _cacheErrorChangedArgs;
        static DataErrorsChangedEventArgs GetErrorChangedArgs(string propertyName)
        {
            if (!_cacheErrorChangedArgs.Keys.Contains(propertyName))
            {
                _cacheErrorChangedArgs[propertyName] = new DataErrorsChangedEventArgs(propertyName);
            }
            return _cacheErrorChangedArgs[propertyName];
        }

        ErrorCollection<string> _errors;
        public bool HasErrors
        {
            get
            {
                return _errors.ErrorCount > 0;
            }
        }
        protected virtual void RaiseErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged(this, GetErrorChangedArgs(propertyName));
            }
        }
        
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;



        static Dictionary<string, PropertyChangedEventArgs> _cachePropertyChangedArgs;

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
