using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using PeakMVP.Helpers;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.Sports;
using PeakMVP.Models.Rests.Responses.Sports;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.RequestProvider;
using Plugin.Connectivity;

namespace PeakMVP.Services.Sports {
    public class SportService : ISportService {

        private readonly IRequestProvider _requestProvider;
        private readonly IIdentityUtilService _identityUtilService;

        private SportDTO[] _sportsList = new SportDTO[] { };

        /// <summary>
        ///     ctor().
        /// </summary>
        public SportService(
            IRequestProvider requestProvider,
            IIdentityUtilService identityUtilService) {

            _requestProvider = requestProvider;
            _identityUtilService = identityUtilService;
        }

        public async Task<List<SportDTO>> GetSportsAsync(CancellationToken cancellationToken) =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                List<SportDTO> sports = null;

                if (_sportsList != null && _sportsList.Any()) {
                    sports = _sportsList.ToList();
                }
                else {
                    sports = new List<SportDTO>();

                    GetSportsRequest getSportsRequest = new GetSportsRequest {
                        AccessToken = GlobalSettings.Instance.UserProfile.AccesToken,
                        Url = GlobalSettings.Instance.Endpoints.SportEndPoints.GetSportsEndPoint
                    };

                    GetSportsResponse getSportsResponse = null;

                    try {
                        getSportsResponse = await _requestProvider.GetAsync<GetSportsRequest, GetSportsResponse>(getSportsRequest);
                        cancellationToken.ThrowIfCancellationRequested();

                        if (getSportsResponse != null && getSportsResponse.Sports.Any()) {
                            sports = getSportsResponse.Sports.ToList();
                            _sportsList = getSportsResponse.Sports.ToArray<SportDTO>();
                        }
                    }
                    catch (ServiceAuthenticationException exc) {
                        _identityUtilService.RefreshToken();

                        throw exc;
                    }
                    catch (Exception ex) {
                        Crashes.TrackError(ex);
                        Debug.WriteLine($"{ex.Message}");
                    }
                }

                return sports;
            }, cancellationToken);
    }
}
