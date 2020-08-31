using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using PeakMVP.Helpers;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.IdentityRequests;
using PeakMVP.Models.Rests.Requests.RequestDataModels;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Identity;
using PeakMVP.Models.Rests.Responses.IdentityResponses;
using PeakMVP.Services.Dialog;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.Navigation;
using PeakMVP.Services.RequestProvider;
using Plugin.Connectivity;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.Services.Identity {
    public sealed class IdentityService : IIdentityService {

        public static readonly string COMMON_RESET_PASSWORD_ERROR = "Can't reset password";
        private static readonly string _COMMON_REGISTRATION_ERROR = "Can't register current user";
        private static readonly string _COMMON_LOGIN_ERROR = "Can't login now";

        private readonly IRequestProvider _requestProvider;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IIdentityUtilService _identityUtilService;

        public IdentityService(
            IRequestProvider requestProvider,
            IDialogService dialogService,
            INavigationService navigationService,
            IIdentityUtilService identityUtilService) {

            _requestProvider = requestProvider;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _identityUtilService = identityUtilService;
        }

        public Task<string> LoginAsync(string userName, string password, CancellationToken cancellationToken) =>
            Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                LoginRequest loginRequest = new LoginRequest {
                    Url = string.Format(GlobalSettings.Instance.Endpoints.AuthenticationEndpoints.LoginEndPoints),
                    Data = new LoginRequestDataModel() {
                        Password = password,
                        Username = userName
                    }
                };

                LoginResponse loginResponse = null;

                try {
                    loginResponse = await _requestProvider.PostAsync<LoginRequest, LoginResponse>(loginRequest);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (HttpRequestExceptionEx exc) {
                    Crashes.TrackError(exc);

                    string output = "";

                    try {
                        LoginResponse bagLoginResponse = JsonConvert.DeserializeObject<LoginResponse>(exc.Message);

                        output = string.Format("{0} {1}",
                            bagLoginResponse.Errors?.FirstOrDefault(),
                            bagLoginResponse.User?.FirstOrDefault().Split('\r').FirstOrDefault());
                    }
                    catch {
                        Debugger.Break();
                        output = _COMMON_LOGIN_ERROR;
                    }

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? _COMMON_LOGIN_ERROR : output;

                    throw new InvalidOperationException(output);
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);

                    string output = "";

                    try {
                        LoginResponse bagLoginResponse = JsonConvert.DeserializeObject<LoginResponse>(ex.Message);

                        output = string.Format("{0} {1}",
                            bagLoginResponse.Errors?.FirstOrDefault(),
                            bagLoginResponse.User?.FirstOrDefault().Split('\r').FirstOrDefault());
                    }
                    catch {
                        Debugger.Break();
                        output = _COMMON_LOGIN_ERROR;
                    }

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? _COMMON_LOGIN_ERROR : output;

                    throw new InvalidOperationException(output);
                }

                return loginResponse?.AccessToken;
            }, cancellationToken);

        public Task<RegistrationResponse> RegistrationAsync(RegistrationRequestDataModel registrationRequestDataModel, CancellationTokenSource cancellationTokenSource) =>
            Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                RegistrationRequest registrationRequest = new RegistrationRequest {
                    Url = string.Format(GlobalSettings.Instance.Endpoints.AuthenticationEndpoints.RegistrationEndPoints),
                    Data = registrationRequestDataModel
                };

                string testJSON = JsonConvert.SerializeObject(registrationRequest);

                RegistrationResponse registrationResponse = null;
                try {
                    registrationResponse = await _requestProvider.PostAsync<RegistrationRequest, RegistrationResponse>(registrationRequest);
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);

                    string output = "";

                    try {
                        RegistrationBadResponse registrationBadResponse = JsonConvert.DeserializeObject<RegistrationBadResponse>(ex.Message);

                        output = string.Format("{0} {1}",
                            registrationBadResponse.Errors?.FirstOrDefault(),
                            registrationBadResponse.DateOfBirth?.FirstOrDefault().Split('\r').FirstOrDefault());
                    }
                    catch (Exception exc) {
                        output = ex.Message;
                    }

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? _COMMON_REGISTRATION_ERROR : output;

                    throw new InvalidOperationException(output.Trim());
                }

                return registrationResponse;
            }, cancellationTokenSource.Token);

        public Task<bool> ResetPasswordAsync(string targetEmail, CancellationTokenSource cancellationTokenSource) =>
            Task<bool>.Run(async () => {
                bool completion = false;

                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                ResetPasswordRequest resetPasswordRequest = new ResetPasswordRequest() {
                    Data = new ResetPasswordDataModel() {
                        Identificator = targetEmail
                    },
                    Url = GlobalSettings.Instance.Endpoints.AuthenticationEndpoints.ResetPassword
                };

                ResetPasswordResponse resetPasswordResponse = null;

                try {
                    resetPasswordResponse = await _requestProvider.PostAsync<ResetPasswordRequest, ResetPasswordResponse>(resetPasswordRequest);

                    completion = resetPasswordResponse != null;
                }
                catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    completion = false;
                    throw exc;
                }
                catch (Exception ex) {
                    Crashes.TrackError(ex);

                    ResetPasswordResponse badResetPasswordResponse = JsonConvert.DeserializeObject<ResetPasswordResponse>(ex.Message);

                    string output = string.Format("{0} {1}",
                        badResetPasswordResponse.Identificator?.FirstOrDefault(),
                        badResetPasswordResponse.Errors?.FirstOrDefault());

                    output = (string.IsNullOrWhiteSpace(output) || string.IsNullOrEmpty(output)) ? COMMON_RESET_PASSWORD_ERROR : output;

                    completion = false;
                    throw new InvalidOperationException(output.Trim());
                }

                return completion;
            }, cancellationTokenSource.Token);
    }
}
