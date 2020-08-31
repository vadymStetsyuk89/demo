using PeakMVP.Validations.Contracts;

namespace PeakMVP.Validations.ValidationRules {
    public class DigitRule<T> : IValidationRule<T> {
        public string ValidationMessage { get; set; }

        public bool Check(T value) {
            if (value == null)
                return false;

            bool hasDigit = false;

            var validationString = value as string;

            foreach (var c in validationString) {
                if (char.IsDigit(c)) {
                    hasDigit = true;
                } else {
                    return false;
                }
            }
            return hasDigit;
        }
    }
}
