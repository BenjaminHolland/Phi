using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Phi.Core.Development;
using System.Runtime.Caching;
using System.CodeDom.Compiler;
namespace Phi.Core {

   
    public static class PropertyHelper {
        static CodeDomProvider _codeDomainProvider = CodeDomProvider.CreateProvider("C#");
        public static void ThrowIfInvalidPropertyName(string propertyName) {
            if (!_codeDomainProvider.IsValidIdentifier(propertyName)) throw new ArgumentException($"{propertyName} is not a valid property name.");
        }
        public static PropertyInfo GetProperty(object target, string propertyName) {
            ThrowIfInvalidPropertyName(propertyName);
            return target.GetType().GetRuntimeProperty(propertyName);
        }
        public static bool HasProperty(object target, string propertyName) {
            return GetProperty(target, propertyName) == null;
        }


        static MemoryCache _cachePropertyChangedArgs;
        public static PropertyChangedEventArgs GetPropertyChangedEventArgs(string propertyName) {
            ThrowIfInvalidPropertyName(propertyName);
            if (!_cachePropertyChangedArgs.Contains(propertyName)) {
                _cachePropertyChangedArgs.Add(propertyName, new PropertyChangedEventArgs(propertyName), DateTimeOffset.Now.AddMinutes(1));
            }
            return _cachePropertyChangedArgs.Get(propertyName) as PropertyChangedEventArgs;
        }
        static MemoryCache _cacheErrorChangedArgs;
        public static DataErrorsChangedEventArgs GetErrorChangedEventArgs(string propertyName) {
            ThrowIfInvalidPropertyName(propertyName);
            if (!_cacheErrorChangedArgs.Contains(propertyName)) {
                _cacheErrorChangedArgs.Add(propertyName, new DataErrorsChangedEventArgs(propertyName), DateTimeOffset.Now.AddMinutes(1));
            }
            return _cacheErrorChangedArgs.Get(propertyName) as DataErrorsChangedEventArgs;
        }

    }
    [CodeSource("Pluralsight Blog", "Unknown", @"http://blog.pluralsight.com/async-validation-wpf-prism")]
    public class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo {
        #region INotifyDataErrorInfo
        protected void ThrowIfInvalidProperty(string propertyName) {
            if (!PropertyHelper.HasProperty(this, propertyName)) throw new ArgumentException("propertyName");
        }
        private Dictionary<string, List<string>> _errors;
        private void EnsureErrorEntry(string propertyName) {
            ThrowIfInvalidProperty(propertyName);
            if (!_errors.ContainsKey(propertyName)) {
                _errors.Add(propertyName, new List<string>());
            }
        }
        public bool Validate(string propertyName) {
            ThrowIfInvalidProperty(propertyName);
            PropertyInfo property = PropertyHelper.GetProperty(this, propertyName);

            List<ValidationResult> results = new List<ValidationResult>();
            ValidationContext context = new ValidationContext(this);
            var value = property.GetValue(this);

            bool isValid = Validator.TryValidateProperty(value, context, results);
            SetErrors(propertyName, results.Select(result => result.ErrorMessage));
            OnDataErrorsChanged(propertyName);
            return isValid;
        }
        public void SetErrors(string propertyName,IEnumerable<string> errors) {
            if (errors == null) throw new ArgumentNullException("errors");
            EnsureErrorEntry(propertyName);
            _errors[propertyName].Clear();
            _errors[propertyName].AddRange(errors);
        }
        public void ClearErrors(string propertyName) {
            EnsureErrorEntry(propertyName);
            _errors[propertyName].Clear();
        }
        public bool HasErrors {
            get {
                return _errors.Select(entry => entry.Value.Count).Sum()!=0;
            }
        }


        public IEnumerable GetErrors(string propertyName) {
            EnsureErrorEntry(propertyName);
            return _errors[propertyName].AsEnumerable();
        }
        protected virtual void OnDataErrorsChanged(string propertyName) {
            ThrowIfInvalidProperty(propertyName);
            if (ErrorsChanged != null) {
                ErrorsChanged(this, PropertyHelper.GetErrorChangedEventArgs(propertyName));
            }
        }
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        #endregion

        #region INotifyPropertyChanged
        protected virtual void OnPropertyChanged(string propertyName) {
            if (!PropertyHelper.HasProperty(this, propertyName)) throw new InvalidOperationException($"{propertyName} is not a valid property.");
            if (PropertyChanged != null) {
                PropertyChanged(this, PropertyHelper.GetPropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        protected virtual bool SetProperty<T>(ref T storage, T value, string propertyName) {
            ThrowIfInvalidProperty(propertyName);
            if (Equals(storage, value)) return false;
            OnPropertyChanged(propertyName);
            Validate(propertyName);
            return true;
        }
    }
}
