using PeakMVP.Models.Rests.Requests.RequestDataModels.Profile;
using PeakMVP.ViewModels.Base;
using System.Threading.Tasks;

namespace PeakMVP.ViewModels.MainContent.ProfileSettings.SelfInformations {
    public interface ISelfInformationViewModel : IInputForm {

        void ResolveProfileInfo();

        SetProfileDataModel GetInputValues();

        void Dispose();

        Task InitializeAsync(object navigationData);
    }
}
