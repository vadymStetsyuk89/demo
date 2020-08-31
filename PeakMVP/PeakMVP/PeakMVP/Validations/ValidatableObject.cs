using PeakMVP.Validations.Contracts;
using PeakMVP.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;

namespace PeakMVP.Validations {
    public class ValidatableObject<T> : ExtendedBindableObject, IValidity {

        public static readonly string VALUE_PROPERTY_NAME = "Value";

        private readonly List<IValidationRule<T>> _validations;

        public List<IValidationRule<T>> Validations => _validations;

        public ValidatableObject() {
            _isValid = true;
            _errors = new List<string>();
            _validations = new List<IValidationRule<T>>();
        }

        List<string> _errors;
        public List<string> Errors {
            get { return _errors; }
            set { SetProperty(ref _errors, value); }
        }

        T _value;
        public T Value {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        bool _isValid;
        public bool IsValid {
            get { return _isValid; }
            set { SetProperty(ref _isValid, value); }
        }

        public bool Validate() {
            Errors.Clear();

            IEnumerable<string> errors = _validations.Where(v => !v.Check(Value))
                .Select(v => v.ValidationMessage);

            Errors = errors.ToList();
            IsValid = !Errors.Any();

            return this.IsValid;
        }
    }
}
