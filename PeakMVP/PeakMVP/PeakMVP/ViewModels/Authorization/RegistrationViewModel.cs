using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.Arguments.InitializeArguments.Authorization;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.Requests.RequestDataModels;
using PeakMVP.Models.Rests.Responses.IdentityResponses;
using PeakMVP.Services.DataItems.Autorization;
using PeakMVP.Services.Identity;
using PeakMVP.ViewModels.Authorization.Registration;
using PeakMVP.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.Authorization {
    public sealed class RegistrationViewModel : ContentPageBaseViewModel {

        private readonly IIdentityService _identityService;
        private readonly ICreateProfileDataItems<ProfileTypeItem> _createProfileDataItems;

        private CancellationTokenSource _registrationCancellationTokenSource = new CancellationTokenSource();

        public RegistrationViewModel(
            ICreateProfileDataItems<ProfileTypeItem> createProfileDataItems,
            IIdentityService identityService) {

            _identityService = identityService;
            _createProfileDataItems = createProfileDataItems;

            ProfileTypeItems = _createProfileDataItems.BuildDataItems();
        }

        public ICommand LoginCommand => new Command(async () => await NavigationService.NavigateToAsync<LoginViewModel>());

        public ICommand CreateCommand => new Command(async () => {
            IsBusy = true;

            IsErrorMessageVisible = false;
            ErrorMessage = "";

            if (TypeSpecificRegistrationInputForm.ValidateForm()) {
                ResetCancellationTokenSource(ref _registrationCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _registrationCancellationTokenSource;

                RegistrationRequestDataModel registrationRequest = TypeSpecificRegistrationInputForm.BuildRegistrationDataModel();

                try {
                    RegistrationResponse registrationResponse = await _identityService.RegistrationAsync(registrationRequest, cancellationTokenSource);

                    if (registrationResponse != null) {

                        NavigationService.Initialize();
                        await NavigationService.NavigateToAsync<LoginViewModel>(new RegisteredArgs() { Email = registrationRequest.Contact.Email, FirstName = registrationRequest.FirstName, LastName = registrationRequest.LastName });
                    }
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    IsErrorMessageVisible = true;
                    ErrorMessage = exc.Message;
                }
            }

            IsBusy = false;
        });

        private IRegistrationInputForm<RegistrationRequestDataModel> _typeSpecificRegistrationInputForm;
        public IRegistrationInputForm<RegistrationRequestDataModel> TypeSpecificRegistrationInputForm {
            get => _typeSpecificRegistrationInputForm;
            private set => SetProperty<IRegistrationInputForm<RegistrationRequestDataModel>>(ref _typeSpecificRegistrationInputForm, value);
        }

        private bool _isErrorMessageVisible;
        public bool IsErrorMessageVisible {
            get => _isErrorMessageVisible;
            set => SetProperty<bool>(ref _isErrorMessageVisible, value);
        }

        private string _errorMessage;
        public string ErrorMessage {
            get => _errorMessage;
            set => SetProperty<string>(ref _errorMessage, value);
        }

        private ProfileTypeItem _selectedProfileTypeItem;
        public ProfileTypeItem SelectedProfileTypeItem {
            get => _selectedProfileTypeItem;
            set {
                ResolveInputForm(value);
                SetProperty<ProfileTypeItem>(ref _selectedProfileTypeItem, value);

                ResetForm();
            }
        }

        IEnumerable<ProfileTypeItem> _profileTypeItems;
        public IEnumerable<ProfileTypeItem> ProfileTypeItems {
            get => _profileTypeItems;
            set => SetProperty<IEnumerable<ProfileTypeItem>>(ref _profileTypeItems, value);
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is ProfileTypeItem) {
                SelectedProfileTypeItem = ProfileTypeItems.FirstOrDefault<ProfileTypeItem>(pI => pI.ProfileType == ((ProfileTypeItem)navigationData).ProfileType);
            }

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            TypeSpecificRegistrationInputForm.Dispose();

            CancellationService.Cancel();
            ResetCancellationTokenSource(ref _registrationCancellationTokenSource);
        }

        private void ResolveInputForm(ProfileTypeItem targetProfile) {
            TypeSpecificRegistrationInputForm?.Dispose();

            switch (targetProfile.ProfileType) {
                case ProfileType.Fan:
                    TypeSpecificRegistrationInputForm = ViewModelLocator.Resolve<FanRegistrationInputFormViewModel>();
                    break;
                case ProfileType.Player:
                    TypeSpecificRegistrationInputForm = ViewModelLocator.Resolve<PlayerRegistrationInputFormViewModel>();
                    break;
                case ProfileType.Parent:
                    TypeSpecificRegistrationInputForm = ViewModelLocator.Resolve<ParentRegistrationInputFormViewModel>();
                    break;
                case ProfileType.Organization:
                    TypeSpecificRegistrationInputForm = ViewModelLocator.Resolve<OrganizationRegistrationInputFormViewModel>();
                    break;
                case ProfileType.Coach:
                    TypeSpecificRegistrationInputForm = ViewModelLocator.Resolve<CoachRegistrationInputFormViewModel>();
                    ((CoachRegistrationInputFormViewModel)TypeSpecificRegistrationInputForm).InitializeAsync(null);
                    break;
                default:
                    Debugger.Break();
                    break;
            }
        }

        private void ResetForm() {
            TypeSpecificRegistrationInputForm?.ResetInputForm();
            IsErrorMessageVisible = false;
            ErrorMessage = null;
        }
    }
}
