using PeakMVP.Validations;

namespace PeakMVP.Factories.Validation {
    public class ValidationObjectFactory : IValidationObjectFactory {
        public ValidatableObject<T> GetValidatableObject<T>() {
            return new ValidatableObject<T>();
        }
    }
}
