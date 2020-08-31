using PeakMVP.Validations.Contracts;
using System;

namespace PeakMVP.Validations.ValidationRules {
    public class DateRule<T> : IValidationRule<T>  {

        public TimeSpan DaysRestriction { get; set; } = TimeSpan.FromDays(366);

        public string ValidationMessage { get; set; }

        public bool Check(T value) {
            TimeSpan currentSpan = DateTime.Now - DateTime.Parse(value.ToString());

            bool result = currentSpan > DaysRestriction;

            return result;
        }
    }
}
