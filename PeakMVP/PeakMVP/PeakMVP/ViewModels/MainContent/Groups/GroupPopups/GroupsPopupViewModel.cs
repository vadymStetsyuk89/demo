using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.Validation;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Identities.Groups;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Groups;
using PeakMVP.Services.Groups;
using PeakMVP.Validations;
using PeakMVP.Validations.ValidationRules;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.Base.Popups;
using PeakMVP.Views.CompoundedViews.MainContent.Groups.Popups;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Groups.GroupPopups {
    public class GroupsPopupViewModel : PopupBaseViewModel, IInputForm {

        private readonly IGroupsService _groupsService;
        private readonly IValidationObjectFactory _validationObjectFactory;

        private CancellationTokenSource _createNewGroupCancellationTokenSource = new CancellationTokenSource();

        public GroupsPopupViewModel(
            IGroupsService groupsService,
            IValidationObjectFactory validationObjectFactory) {

            _groupsService = groupsService;
            _validationObjectFactory = validationObjectFactory;
        }

        public ICommand SaveNewGroupCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            if (ValidateForm()) {
                ResetCancellationTokenSource(ref _createNewGroupCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _createNewGroupCancellationTokenSource;

                try {
                    GroupDTO createdGroup = await _groupsService.CreateNewGroupAsync(new CreateGroupDataModel() {
                        GroupType = GroupType.Regular.ToString(),
                        Name = GroupName.Value
                    }, cancellationTokenSource);

                    if (createdGroup != null) {
                        ((ContentPageBaseViewModel)NavigationService.LastPageViewModel).IsPopupsVisible = false;

                        MessagingCenter.Send<object, GroupDTO>(this, GlobalSettings.Instance.AppMessagingEvents.GroupsEvents.NewGroupCreated, createdGroup);
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

            UpdateBusyVisualState(busyKey, false);
        });

        public override Type RelativeViewType => typeof(AddGroupPopup);

        private ValidatableObject<string> _groupName;
        public ValidatableObject<string> GroupName {
            get => _groupName;
            set => SetProperty<ValidatableObject<string>>(ref _groupName, value);
        }

        public void ResetInputForm() {
            ResetValidationObjects();
        }

        public bool ValidateForm() {
            bool isValidResult = false;

            GroupName.Validate();

            ///
            /// xxx.IsValid && yyy.IsValid && zzz.IsValid
            /// 
            isValidResult = GroupName.IsValid;

            return isValidResult;
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _createNewGroupCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _createNewGroupCancellationTokenSource);
        }

        public override Task InitializeAsync(object navigationData) {

            if (navigationData is GroupsViewModel) {
                ResetInputForm();
            }

            return base.InitializeAsync(navigationData);
        }

        protected override void OnIsPopupVisible() {
            base.OnIsPopupVisible();

            ResetInputForm();
        }

        private void ResetValidationObjects() {
            GroupName = _validationObjectFactory.GetValidatableObject<string>();
            GroupName.Validations.Add(new IsNotNullOrEmptyRule<string>() { ValidationMessage = IsNotNullOrEmptyRule<LocationDTO>.FIELD_IS_REQUIRED_ERROR_MESSAGE });
        }
    }
}
