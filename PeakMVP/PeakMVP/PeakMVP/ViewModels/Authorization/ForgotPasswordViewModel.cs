using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.Validation;
using PeakMVP.Models.Arguments.InitializeArguments.Authorization;
using PeakMVP.Models.Exceptions;
using PeakMVP.Services.Identity;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using PeakMVP.ViewModels.Base;
using System;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.Authorization {
    public class ForgotPasswordViewModel : ContentPageBaseViewModel, IInputForm {

        protected readonly IValidationObjectFactory _validationObjectFactory;
        private readonly IIdentityService _identityService;

        private CancellationTokenSource _resetPasswordCancellationTokenSource = new CancellationTokenSource();

        public ForgotPasswordViewModel(
            IValidationObjectFactory validationObjectFactory,
            IIdentityService identityService) {

            _validationObjectFactory = validationObjectFactory;
            _identityService = identityService;

            ResetValidationObjects();
        }

        public ICommand LoginCommand => new Command(async () => await NavigationService.NavigateToAsync<LoginViewModel>());

        public ICommand ResetPasswordCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            SetBusy(busyKey, true);

            if (ValidateForm()) {
                ResetCancellationTokenSource(ref _resetPasswordCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _resetPasswordCancellationTokenSource;

                try {
                    bool resetCompletion = await _identityService.ResetPasswordAsync(Email.Value, cancellationTokenSource);

                    if (resetCompletion) {
                        await NavigationService.NavigateToAsync<LoginViewModel>(new PasswordResetArgs() { TargetEmail = Email.Value });
                    }
                    else {
                        throw new InvalidOperationException(IdentityService.COMMON_RESET_PASSWORD_ERROR);
                    }
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }
            }

            SetBusy(busyKey, false);
        });

        private ValidatableObject<string> _email;
        public ValidatableObject<string> Email {
            get => _email;
            set => SetProperty<ValidatableObject<string>>(ref _email, value);
        }

        public void ResetInputForm() {
            ResetValidationObjects();
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _resetPasswordCancellationTokenSource);
        }

        public virtual bool ValidateForm() {
            bool isValidResult = false;

            Email.Validate();

            isValidResult = Email.IsValid;

            return isValidResult;
        }

        private void ResetValidationObjects() {
            Email = _validationObjectFactory.GetValidatableObject<string>();
            Email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
            Email.Validations.Add(new EmailRule<string> { ValidationMessage = EmailRule<string>.INVALID_EMAIL_ERROR_MESSAGE });

//#if DEBUG
//            Email.Value = "@mailinator.com";
//#endif
        }
    }
}
