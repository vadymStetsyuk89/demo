using PeakMVP.Validations.Contracts;
using System.Text.RegularExpressions;

namespace PeakMVP.Validations.ValidationRules {
    public class EmailRule<T> : IValidationRule<T> {

        public static readonly string INVALID_EMAIL_ERROR_MESSAGE = "Invalid email";

        public string ValidationMessage { get; set; }

        public bool Check(T value) {
            if (value == null)
                return false;

            string validatedValue = value as string;

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(validatedValue);

            return match.Success;
        }
    }
}
