using PeakMVP.Models.Rests.Requests.RequestDataModels;
using PeakMVP.Models.Rests.Responses.IdentityResponses;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.Identity {
    public interface IIdentityService {

        Task<bool> ResetPasswordAsync(string targetEmail, CancellationTokenSource cancellationTokenSource);

        Task<RegistrationResponse> RegistrationAsync(RegistrationRequestDataModel registrationRequestDataModel, CancellationTokenSource cancellationTokenSource);

        Task<string> LoginAsync(string userName, string password, CancellationToken cancellationToken);
    }
}
