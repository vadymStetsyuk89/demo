using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Factories.MainContent;
using PeakMVP.Factories.Validation;
using PeakMVP.Helpers;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Identities.Groups;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.Groups;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.ActionBars.Top;
using PeakMVP.ViewModels.MainContent.Groups.Arguments;
using PeakMVP.ViewModels.MainContent.Groups.GroupPopups;
using PeakMVP.ViewModels.MainContent.Invites;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Groups {
    public class GroupInfoViewModel : LoggedContentPageViewModel {

        private static readonly string _DELETE_GROUP_BY_ID_COMMON_ERROR_MESSAGE = "Can't delete {0} group";
        private static readonly string _GET_GROUP_BY_ID_COMMON_ERROR_MESSAGE = "Can't found {0} group";

        private readonly IValidationObjectFactory _validationObjectFactory;
        private readonly IGroupsService _groupsService;
        private readonly IGroupingFactory _groupingFactory;
        private readonly IStateService _stateService;

        private CancellationTokenSource _deleteGroupCancelationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _getGroupByIdCancellationTokenSource = new CancellationTokenSource();

        public GroupInfoViewModel(
            IValidationObjectFactory validationObjectFactory,
            IGroupsService groupsService,
            IGroupingFactory groupingFactory,
            IStateService stateService) {

            _validationObjectFactory = validationObjectFactory;
            _groupsService = groupsService;
            _groupingFactory = groupingFactory;
            _stateService = stateService;

            AppBackgroundImage = GlobalSettings.Instance.UserProfile.AppBackgroundImage?.Url;

            ActionBarViewModel = ViewModelLocator.Resolve<ModeActionBarViewModel>();
            ActionBarViewModel.InitializeAsync(this);
            ((ModeActionBarViewModel)ActionBarViewModel).IsModesAvailable = false;

            AddMemberToGroupPopupViewModel = ViewModelLocator.Resolve<AddMemberToGroupPopupViewModel>();
            AddMemberToGroupPopupViewModel.InitializeAsync(this);

            RefreshCommand = new Command(async() => {
                IsRefreshing = true;

                InitializeGroupDTO(TargetGroup.Id);

                await Task.Delay(AppConsts.DELAY_STUB);
                IsRefreshing = false;
            });
        }

        public ICommand RemoveGroupCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            SetBusy(busyKey, true);

            ResetCancellationTokenSource(ref _deleteGroupCancelationTokenSource);
            CancellationTokenSource cancellationTokenSource = _deleteGroupCancelationTokenSource;

            try {
                bool deleteCompletion = await _groupsService.DeleteGroupByIdAsync(TargetGroup.Id, cancellationTokenSource);

                if (deleteCompletion) {
                    MessagingCenter.Instance.Send<object, GroupDTO>(this, GlobalSettings.Instance.AppMessagingEvents.GroupsEvents.GroupDeleted, TargetGroup);

                    await NavigationService.GoBackAsync();
                }
                else {
                    await DialogService.ToastAsync(string.Format(GroupInfoViewModel._DELETE_GROUP_BY_ID_COMMON_ERROR_MESSAGE, TargetGroup.Name));
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }

            SetBusy(busyKey, false);
        });

        public ICommand ShowAddMemberToGroupPopupCommand => new Command(() => AddMemberToGroupPopupViewModel.IsPopupVisible = IsPopupsVisible = true);

        public ICommand RemoveGroupMemberCommand => new Command(async (object param) => await DialogService.ToastAsync(string.Format("{0} {1} remove in developing.", ((Member)param).Profile.FirstName, ((Member)param).Profile.LastName)));

        private AddMemberToGroupPopupViewModel _addMemberToGroupPopupViewModel;
        public AddMemberToGroupPopupViewModel AddMemberToGroupPopupViewModel {
            get => _addMemberToGroupPopupViewModel;
            private set {
                _addMemberToGroupPopupViewModel?.Dispose();

                SetProperty<AddMemberToGroupPopupViewModel>(ref _addMemberToGroupPopupViewModel, value);
            }
        }

        private GroupDTO _targetGroup;
        public GroupDTO TargetGroup {
            get => _targetGroup;
            private set => SetProperty<GroupDTO>(ref _targetGroup, value);
        }

        private ObservableCollection<Member> _groupMembers = new ObservableCollection<Member>();
        public ObservableCollection<Member> GroupMembers {
            get => _groupMembers;
            private set => SetProperty<ObservableCollection<Member>>(ref _groupMembers, value);
        }

        private bool _isRemoveButtonEnabled;
        public bool IsRemoveButtonEnabled {
            get => _isRemoveButtonEnabled;
            private set => SetProperty<bool>(ref _isRemoveButtonEnabled, value);
        }

        private bool _isActionsAvalable;
        public bool IsActionsAvalable {
            get => _isActionsAvalable;
            private set => SetProperty<bool>(ref _isActionsAvalable, value);
        }

        public override void Dispose() {
            base.Dispose();

            AddMemberToGroupPopupViewModel?.Dispose();

            ResetCancellationTokenSource(ref _deleteGroupCancelationTokenSource);
            ResetCancellationTokenSource(ref _getGroupByIdCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _deleteGroupCancelationTokenSource);
            ResetCancellationTokenSource(ref _getGroupByIdCancellationTokenSource);
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is GroupDTO) {
                InitializeGroupDTO(((GroupDTO)navigationData).Id);
            }
            else if (navigationData is GroupInviteItemViewModel requestItemViewModel) {
                InitializeGroupDTO(requestItemViewModel.GroupId);
            }

            AddMemberToGroupPopupViewModel?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        protected override void OnSubscribeOnAppEvents() {
            base.OnSubscribeOnAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.AppBackgroundImageChanged += OnProfileSettingsEventsAppBackgroundImageChanged;
            _stateService.ChangedGroups += OnStateServiceChangedGroups;
            _stateService.ChangedProfile += OnStateServiceChangedProfile;
        }

        protected override void OnUnsubscribeFromAppEvents() {
            base.OnUnsubscribeFromAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.AppBackgroundImageChanged -= OnProfileSettingsEventsAppBackgroundImageChanged;
            _stateService.ChangedGroups -= OnStateServiceChangedGroups;
            _stateService.ChangedProfile -= OnStateServiceChangedProfile;
        }

        private async void InitializeGroupDTO(long groupId) {
            Guid busyKey = Guid.NewGuid();
            SetBusy(busyKey, true);

            ResetCancellationTokenSource(ref _getGroupByIdCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _getGroupByIdCancellationTokenSource;

            try {
                GroupDTO foundGroupDTO = await _groupsService.GetGroupByIdAsync(groupId, cancellationTokenSource);

                if (foundGroupDTO != null) {
                    TargetGroup = foundGroupDTO;

                    IsRemoveButtonEnabled = (TargetGroup.OwnerId == GlobalSettings.Instance.UserProfile.Id);
                    IsActionsAvalable = (TargetGroup.OwnerId == GlobalSettings.Instance.UserProfile.Id);

                    GroupMembers = _groupingFactory.BuildGroupMembers(TargetGroup.Members, TargetGroup.OwnerId, IsActionsAvalable).ToObservableCollection();
                }
                else {
                    await DialogService.ToastAsync(string.Format(GroupInfoViewModel._GET_GROUP_BY_ID_COMMON_ERROR_MESSAGE, TargetGroup.Name));
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }

            SetBusy(busyKey, false);
        }

        private void OnStateServiceChangedGroups(object sender, ChangedGroupsSignalArgs e) {
            if (!e.Data.Any(g => g.Id == TargetGroup.Id)) {
                OnUnsubscribeFromAppEvents();
                Dispose();
                Device.BeginInvokeOnMainThread(async () => await NavigationService.GoBackAsync());
            }
            else {
                InitializeGroupDTO(TargetGroup.Id);
            }
        }

        private void OnProfileSettingsEventsAppBackgroundImageChanged(object sender, EventArgs e) => AppBackgroundImage = GlobalSettings.Instance.UserProfile.AppBackgroundImage?.Url;

        private async void OnStateServiceChangedProfile(object sender, ChangedProfileSignalArgs e) {
            try {
                InitializeGroupDTO(TargetGroup.Id);
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);
                await DialogService.ToastAsync("Can't resolve groups, after profile update");
                Device.BeginInvokeOnMainThread(async () => await NavigationService.GoBackAsync());
            }
        }
    }
}
