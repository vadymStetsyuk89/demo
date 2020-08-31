using PeakMVP.ViewModels.Base;

namespace PeakMVP.ViewModels.Authorization.Registration {
    public interface IRegistrationInputForm<TRegistrationDTO> : IInputForm {

        string LoginDetailsTitle { get; }

        TRegistrationDTO BuildRegistrationDataModel();

        void Dispose();
    }
}
