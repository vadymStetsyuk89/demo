using PeakMVP.Validations.Contracts;

namespace PeakMVP.Validations.ValidationRules {
    public class MinLengthRule<T> : IValidationRule<T> {

        public static readonly string MIN_LENGTH_ERROR_MESSAGE = "Min. {0} characters";
        public static readonly int MIN_DEFAULT_VALUE = 3;

        public string ValidationMessage { get; set; }

        public int MinLength { get; set; } = MIN_DEFAULT_VALUE;

        public bool IsCanBeEmpty { get; set; }

        public bool Check(T value) {
            string stringValue = value as string;

            if (IsCanBeEmpty) {
                if (string.IsNullOrEmpty(stringValue)) {
                    return true;
                }
                else {
                    return CheckLength(stringValue);
                }
            }
            else {
                return CheckLength(stringValue);
            }
        }

        private bool CheckLength(string value) {
            if (!string.IsNullOrEmpty(value)) {
                return value.ToString().Length >= MinLength;
            }
            else {
                return false;
            }
        }
    }
}
