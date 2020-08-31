using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using PeakMVP.Helpers;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.Family;
using PeakMVP.Models.Rests.Responses.Family;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.RequestProvider;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.Family {
    public class FamilyService : IFamilyService {

        private static readonly string GET_FAMILY_COMMON_ERROR = "Can't get family info now";

        private readonly IRequestProvider _requestProvider;
        private readonly IIdentityUtilService _identityUtilService;

        public FamilyService(
            IRequestProvider requestProvider,
            IIdentityUtilService identityUtilService) {

            _requestProvider = requestProvider;
            _identityUtilService = identityUtilService;
        }

        public Task<FamilyDTO> GetFamilyAsync(CancellationTokenSource cancellationTokenSource) =>
            Task<FamilyDTO>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                FamilyDTO familyDTO = null;

                GetFamilyRequest getFamilyRequest = new GetFamilyRequest() {
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                    Url = GlobalSettings.Instance.Endpoints.FamilyEndpoints.GetFamilyEndpoint
                };

                try {
                    GetFamilyResponse getFamilyResponse = await _requestProvider.GetAsync<GetFamilyRequest, GetFamilyResponse>(getFamilyRequest);

                    if (getFamilyResponse != null) {
                        familyDTO = new FamilyDTO() {
                            Id = getFamilyResponse.Id,
                            Members = getFamilyResponse.Members,
                            ParentId = getFamilyResponse.ParentId
                        };
                    }
                } catch (HttpRequestExceptionEx exc) {
                    GetFamilyResponse erroredGetFamilyResponse = JsonConvert.DeserializeObject<GetFamilyResponse>(exc.Message);

                    string output = string.Format("{0}",
                                    erroredGetFamilyResponse.Errors?.FirstOrDefault());

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? GET_FAMILY_COMMON_ERROR : output;

                    throw new InvalidOperationException(output.Trim());
                } catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                } catch (Exception exc) {
                    Crashes.TrackError(exc);

                    throw exc;
                }

                return familyDTO;
            });
    }
}
