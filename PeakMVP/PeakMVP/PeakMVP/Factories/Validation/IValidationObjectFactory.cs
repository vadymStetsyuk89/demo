using PeakMVP.Validations;

namespace PeakMVP.Factories.Validation {
    public interface IValidationObjectFactory {
        ValidatableObject<T> GetValidatableObject<T>();
    }
}
