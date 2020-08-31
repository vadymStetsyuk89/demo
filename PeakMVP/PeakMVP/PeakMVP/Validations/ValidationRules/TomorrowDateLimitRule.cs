using PeakMVP.Validations.Contracts;
using System;

namespace PeakMVP.Validations.ValidationRules {
    public class TomorrowDateLimitRule<T>: IValidationRule<T> {

        public static readonly string DATE_LIMIT_VALUE_ERROR_MESSAGE = "Date limit value is {0:dd MMM yy}";

        public string ValidationMessage { get; set; }

        public bool Check(T value) {
            TimeSpan timeSpan = (DateTime.Now.AddDays(1)) - DateTime.Parse(value.ToString());

            bool result = ((int)timeSpan.TotalDays) > 0;
            
            return result;
        }
    }
}
