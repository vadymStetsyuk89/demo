using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Factories.MainContent;
using PeakMVP.Factories.Validation;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Teams;
using PeakMVP.Models.Rests.Responses.Teams;
using PeakMVP.Services.Sports;
using PeakMVP.Services.Teams;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.Base.Popups;
using PeakMVP.Views.CompoundedViews.MainContent.ProfileContent.Popups;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.ProfileContent.Popups {
    public class AddTeamPopupViewModel : PopupBaseViewModel, IAskToRefresh, IInputForm {

        private readonly ITeamService _teamService;
        private readonly IValidationObjectFactory _validationObjectFactory;
        private readonly ISportService _sportService;
        private readonly ISportsFactory _sportsFactory;
        private readonly ITeamFactory _teamFactory;

        private CancellationTokenSource _createNewTeamCancellationTokenSource = new CancellationTokenSource();

        public AddTeamPopupViewModel(
            ITeamService teamService,
            IValidationObjectFactory validationObjectFactory,
            ISportService sportService,
            ISportsFactory sportsFactory,
            ITeamFactory teamFactory) {

            _teamService = teamService;
            _validationObjectFactory = validationObjectFactory;
            _sportService = sportService;
            _sportsFactory = sportsFactory;
            _teamFactory = teamFactory;

            ResetValidationObjects();
        }

        public ICommand SaveNewTeamCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            ((ContentPageBaseViewModel)NavigationService.LastPageViewModel).SetBusy(busyKey, true);

            if (ValidateForm()) {
                ResetCancellationTokenSource(ref _createNewTeamCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _createNewTeamCancellationTokenSource;

                try {
                    CreateTeamDataModel createTeamDataModel = new CreateTeamDataModel {
                        Name = TeamName.Value,
                        SportId = SelectedSportsItem.Id
                    };

                    CreateTeamResponse createTeamResponse = await _teamService.CreateTeamAsync(createTeamDataModel, cancellationTokenSource.Token);

                    if (createTeamResponse != null && createTeamResponse.Owner != null) {

                        GlobalSettings.Instance.AppMessagingEvents.TeamEvents.NewTeamCreatedInvoke(this, _teamFactory.BuildTeam(createTeamResponse));

                        ((ContentPageBaseViewModel)NavigationService.LastPageViewModel).IsPopupsVisible = false;
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

            ((ContentPageBaseViewModel)NavigationService.LastPageViewModel).SetBusy(busyKey, false);
        });

        public override Type RelativeViewType => typeof(OrganizationAddNewTeamPopup);

        private ValidatableObject<string> _teamName;
        public ValidatableObject<string> TeamName {
            get => _teamName;
            set => SetProperty<ValidatableObject<string>>(ref _teamName, value);
        }

        IEnumerable<SportsDataItem> _sportsItems;
        public IEnumerable<SportsDataItem> SportsItems {
            get => _sportsItems;
            set => SetProperty(ref _sportsItems, value);
        }

        SportsDataItem _selectedSportsItem;
        public SportsDataItem SelectedSportsItem {
            get => _selectedSportsItem;
            set => SetProperty(ref _selectedSportsItem, value);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _createNewTeamCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _createNewTeamCancellationTokenSource);
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is OrganizationProfileContentViewModel || navigationData is CoachProfileContentViewModel) {
                GetSportsAsync(CancellationService.GetToken());

                ResetInputForm();
            }

            return base.InitializeAsync(navigationData);
        }

        public Task AskToRefreshAsync() =>
            Task.Run(() => {
                GetSportsAsync(CancellationService.GetToken());
            });

        public void ResetInputForm() {
            ResetValidationObjects();

            SelectedSportsItem = SportsItems?.FirstOrDefault();
        }

        public bool ValidateForm() {
            bool isValidResult = false;

            TeamName.Validate();

            ///
            /// xxx.IsValid && yyy.IsValid && zzz.IsValid
            /// 
            isValidResult = TeamName.IsValid;

            return isValidResult;
        }

        protected override void OnIsPopupVisible() {
            base.OnIsPopupVisible();

            if (!IsPopupVisible) {
                ResetInputForm();
            }

            Dispose();
        }

        private async void GetSportsAsync(CancellationToken cancellationToken) {
            try {
                await Task.Run(async () => {
                    // Get sports.
                    var sports = await _sportService.GetSportsAsync(cancellationToken);
                    if (sports.Any()) {
                        SportsItems = (_sportsFactory.CreateDataItems(sports)).ToObservableCollection();

                        SelectedSportsItem = SportsItems.First();
                    }
                }, cancellationToken);
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                Debug.WriteLine($"ERROR:{exc.Message}");
            }
        }

        private void ResetValidationObjects() {
            TeamName = _validationObjectFactory.GetValidatableObject<string>();
            TeamName.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
        }
    }
}
