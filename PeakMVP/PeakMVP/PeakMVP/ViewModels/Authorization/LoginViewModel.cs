using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.Validation;
using PeakMVP.Helpers;
using PeakMVP.Models.Arguments.InitializeArguments.Authorization;
using PeakMVP.Models.Exceptions;
using PeakMVP.Services.Identity;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.Profile;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using PeakMVP.ViewModels.Base;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.Authorization {
    public sealed class LoginViewModel : ContentPageBaseViewModel {

        private static readonly string[] USER_MESSAGE_SPAN_ARRAY = new[] { "We've sent a message to ", " with instructions for your account activation." };
        private static readonly string[] RESSET_PASSWORD_MESSAGE_SPAN_ARRAY = new[] { "We've sent a message to ", " with instructions for your password recovery" };
        private static readonly string USER_NAME_COLOR_HEX = "#001D2C";

        private readonly IProfileService _profileService;
        private readonly IIdentityUtilService _identityUtilService;
        private readonly IIdentityService _identityService;

        public LoginViewModel(
            IProfileService profileService,
            IIdentityService identityService,
            IValidationObjectFactory validationObjectFactory,
            IIdentityUtilService identityUtilService) {

            _profileService = profileService;
            _identityService = identityService;
            _identityUtilService = identityUtilService;

            _user = validationObjectFactory.GetValidatableObject<string>();
            _password = validationObjectFactory.GetValidatableObject<string>();

            AddValidations();

            //#if DEBUG
            //            User.Value = "todd";
            //            Password.Value = "rlrl240";
            //#endif
        }

        public ICommand SignUpCommand => new Command(async () => await NavigationService.NavigateToAsync<CreateProfileViewModel>());

        public ICommand ForgotPasswordCommand => new Command(async () => await NavigationService.NavigateToAsync<ForgotPasswordViewModel>());

        public ICommand LoginCommand => new Command(async () => await LoginAsync(CancellationService.GetToken()));

        public ICommand ValidateUserCommand => new Command(() => ValidateUser());

        public ICommand ValidatePasswordCommand => new Command(() => ValidatePassword());

        public ICommand CloseUserMessageCcommand => new Command(() => IsUserMessageVisible = false);

        FormattedString _userMessage;
        public FormattedString UserMessage {
            get => _userMessage;
            set => SetProperty(ref _userMessage, value);
        }

        bool _isUserMessageVisible;
        public bool IsUserMessageVisible {
            get => _isUserMessageVisible;
            set => SetProperty<bool>(ref _isUserMessageVisible, value);
        }

        ValidatableObject<string> _user;
        public ValidatableObject<string> User {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        ValidatableObject<string> _password;
        public ValidatableObject<string> Password {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is RegisteredArgs registeredArgs) {
                FormattedString formattedString = new FormattedString();
                formattedString.Spans.Add(new Span() { Text = USER_MESSAGE_SPAN_ARRAY[0] });
                formattedString.Spans.Add(new Span() {
                    Text = registeredArgs.Email,
                    ForegroundColor = Color.FromHex(USER_NAME_COLOR_HEX)
                });
                formattedString.Spans.Add(new Span() { Text = USER_MESSAGE_SPAN_ARRAY[1] });

                UserMessage = formattedString;
                IsUserMessageVisible = true;
            }
            else if (navigationData is PasswordResetArgs passwordResetArgs) {
                FormattedString formattedString = new FormattedString();
                formattedString.Spans.Add(new Span() { Text = RESSET_PASSWORD_MESSAGE_SPAN_ARRAY[0] });
                formattedString.Spans.Add(new Span() {
                    Text = passwordResetArgs.TargetEmail,
                    ForegroundColor = Color.FromHex(USER_NAME_COLOR_HEX)
                });
                formattedString.Spans.Add(new Span() { Text = RESSET_PASSWORD_MESSAGE_SPAN_ARRAY[1] });

                UserMessage = formattedString;
                IsUserMessageVisible = true;
            }

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            CancellationService.Cancel();
        }

        private async Task LoginAsync(CancellationToken cancellationToken) {
            Guid busyKey = Guid.NewGuid();
            SetBusy(busyKey, true);

            if (Validate()) {
                try {
                    await Task.Run(async () => {
                        string accesToken = await _identityService.LoginAsync(User.Value, Password.Value, cancellationToken);

                        GlobalSettings.Instance.UserProfile.AccesToken = accesToken;
                        // Set user profile.
                        if (!string.IsNullOrEmpty(GlobalSettings.Instance.UserProfile.AccesToken)) {

                            await _identityUtilService.ChargeUserProfileAsync(await _profileService.GetProfileAsync(), true);
                        }

                    }, cancellationToken);

                    if (!string.IsNullOrEmpty(GlobalSettings.Instance.UserProfile.AccesToken)) {
                        cancellationToken.ThrowIfCancellationRequested();
                        NavigationService.Initialize(true);
                    }
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception ex) {
                    Crashes.TrackError(ex);

                    await DialogService.ToastAsync(ex.Message);
                }
            }

            SetBusy(busyKey, false);
        }

        private bool Validate() {
            return ValidateUser() && ValidatePassword();
        }

        private bool ValidatePassword() {
            return _password.Validate();
        }

        private bool ValidateUser() {
            return _user.Validate();
        }

        private void AddValidations() {
            _user.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = AppConsts.ERROR_FIELD_REQUIRED });

            _password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = AppConsts.ERROR_FIELD_REQUIRED });
        }
    }
}
