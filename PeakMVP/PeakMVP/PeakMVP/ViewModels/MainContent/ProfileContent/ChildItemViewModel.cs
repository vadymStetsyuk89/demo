using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Services.Profile;
using PeakMVP.ViewModels.Base;
using System;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.ProfileContent {
    public sealed class ChildItemViewModel : NestedViewModel {

        private readonly IProfileService _profileService;

        private CancellationTokenSource _impersonateLogincancellationTokenSource = new CancellationTokenSource();

        public ChildItemViewModel(IProfileService profileService) {
            _profileService = profileService;
        }

        public ICommand LoginImpersonateCommand => new Command(async () => {

            if (IsImpersonateLoginEnabled) {
                ContentPageBaseViewModel contentPageBaseViewModel = ((ContentPageBaseViewModel)NavigationService.LastPageViewModel);
                contentPageBaseViewModel.IsBusy = true;

                ResetCancellationTokenSource(ref _impersonateLogincancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _impersonateLogincancellationTokenSource;

                try {
                    await _profileService.ImpersonateLoginAsync(Id, cancellationTokenSource);
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }

                contentPageBaseViewModel.IsBusy = false;
            }
        });

        public ICommand AddEmailCommand => new Command(async () => await DialogService.ToastAsync("Add email command in developing"));

        public long Id { get; set; }

        private ProfileDTO _profile;
        public ProfileDTO Profile {
            get => _profile;
            set => SetProperty<ProfileDTO>(ref _profile, value);
        }

        string _name;
        public string Name {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        string _age;
        public string Age {
            get { return _age; }
            set { SetProperty(ref _age, value); }
        }

        string _team;
        public string Team {
            get { return _team; }
            set { SetProperty(ref _team, value); }
        }

        private bool _isImpersonateLoginEnabled;
        public bool IsImpersonateLoginEnabled {
            get => _isImpersonateLoginEnabled;
            set => SetProperty<bool>(ref _isImpersonateLoginEnabled, value);
        }

        private bool _isAddEmailCommandEnabled;
        public bool IsAddEmailCommandEnabled {
            get => _isAddEmailCommandEnabled;
            set => SetProperty<bool>(ref _isAddEmailCommandEnabled, value);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _impersonateLogincancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _impersonateLogincancellationTokenSource);
        }
    }
}
