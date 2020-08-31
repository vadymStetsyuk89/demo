using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Factories.MainContent;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.Requests.RequestDataModels;
using PeakMVP.Services.Sports;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.ViewModels.Authorization.Registration {
    public class CoachRegistrationInputFormViewModel : CommonRegistrationInputFormViewModel {

        private readonly ISportService _sportService;
        private readonly ISportsFactory _sportsFactory;

        private CancellationTokenSource _getSportsCancellationTokenSource = new CancellationTokenSource();

        public CoachRegistrationInputFormViewModel(
            ISportsFactory sportsFactory,
            ISportService sportService) {

            _sportService = sportService;
            _sportsFactory = sportsFactory;
        }

        private ValidatableObject<string> _teamName;
        public ValidatableObject<string> TeamName {
            get => _teamName;
            set => SetProperty<ValidatableObject<string>>(ref _teamName, value);
        }

        private SportsDataItem _selectedSportsItem;
        public SportsDataItem SelectedSportsItem {
            get => _selectedSportsItem;
            set => SetProperty<SportsDataItem>(ref _selectedSportsItem, value);
        }

        IEnumerable<SportsDataItem> _sportsItems;
        public IEnumerable<SportsDataItem> SportsItems {
            get => _sportsItems;
            set => SetProperty<IEnumerable<SportsDataItem>>(ref _sportsItems, value);
        }

        public override Task InitializeAsync(object navigationData) {
            GetSports();

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _getSportsCancellationTokenSource);
        }

        public override RegistrationRequestDataModel BuildRegistrationDataModel() {
            RegistrationRequestDataModel registrationRequestDataModel = base.BuildRegistrationDataModel();
            registrationRequestDataModel.Type = ProfileType.Coach.ToString();
            registrationRequestDataModel.Sport = SelectedSportsItem?.Name;
            registrationRequestDataModel.SportId = SelectedSportsItem?.Id.ToString();
            registrationRequestDataModel.Team = TeamName.Value;

            return registrationRequestDataModel;
        }

        public override bool ValidateForm() {
            bool isValidResult = base.ValidateForm();

            if (SelectedSportsItem != null) {
                TeamName.Validate();
            }

            isValidResult = isValidResult && TeamName.IsValid;

            return isValidResult;
        }

        protected override void ResetValidationObjects() {
            base.ResetValidationObjects();

            TeamName = _validationObjectFactory.GetValidatableObject<string>();
            //TeamName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = IsNotNullOrEmptyRule<string>.FIELD_IS_REQUIRED_ERROR_MESSAGE });

            DateOfBirth.Value = new DateTime((DateTime.Now - TimeSpan.FromDays(_DAYS_PER_YEAR * _ADULT_AGE_RESTRICTION)).Ticks);
            DateOfBirth.Validations.Add(new DateRule<DateTime> { ValidationMessage = _TO_YOUNG_ERROR_MESSAGE, DaysRestriction = TimeSpan.FromDays(_DAYS_PER_YEAR * _ADULT_AGE_RESTRICTION) });
        }

        public override void ResetInputForm() {
            base.ResetInputForm();

            SelectedSportsItem = SportsItems?.FirstOrDefault();
        }

        private async void GetSports() {
            ResetCancellationTokenSource(ref _getSportsCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _getSportsCancellationTokenSource;

            try {
                var sports = await _sportService.GetSportsAsync(cancellationTokenSource.Token);

                SportsItems = (_sportsFactory.CreateDataItems(sports)).ToObservableCollection();
                SelectedSportsItem = SportsItems.FirstOrDefault();

                cancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                Debugger.Break();
            }
        }
    }
}
