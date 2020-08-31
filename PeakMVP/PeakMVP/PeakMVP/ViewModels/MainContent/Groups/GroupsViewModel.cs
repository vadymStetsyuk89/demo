using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.MainContent;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.Groups;
using PeakMVP.Services.Navigation;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Groups.GroupPopups;
using PeakMVP.ViewModels.MainContent.Invites;
using PeakMVP.Views.CompoundedViews.MainContent.Groups;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Groups {
    public class GroupsViewModel : TabbedViewModel {

        private readonly IGroupsService _groupsService;
        private readonly IStateService _stateService;

        private CancellationTokenSource _getGroupsCancellationTokenSource = new CancellationTokenSource();

        public GroupsViewModel(
            IGroupsService groupsService,
            IStateService stateService) {

            _groupsService = groupsService;
            _stateService = stateService;

            GroupPopup = ViewModelLocator.Resolve<GroupsPopupViewModel>();
            GroupPopup.InitializeAsync(this);

            IsNestedPullToRefreshEnabled = true;
        }

        public ICommand SelectGroupItemCommand => new Command(async (object param) => await NavigationService.NavigateToAsync<GroupInfoViewModel>(param));

        GroupInviteItemViewModel _selectedItem;
        public GroupInviteItemViewModel SelectedItem {
            get { return _selectedItem; }
            set {
                if (SetProperty(ref _selectedItem, value)) {
                    if (value != null) {
                        OnSelectedItem(value);
                    }

                    SelectedItem = null;
                }
            }
        }

        private GroupsPopupViewModel _groupPopup;
        public GroupsPopupViewModel GroupPopup {
            get => _groupPopup;
            private set => SetProperty<GroupsPopupViewModel>(ref _groupPopup, value);
        }

        private ObservableCollection<GroupDTO> _foundTeams = new ObservableCollection<GroupDTO>();
        public ObservableCollection<GroupDTO> FoundTeams {
            get => _foundTeams;
            set => SetProperty<ObservableCollection<GroupDTO>>(ref _foundTeams, value);
        }

        protected override Task NestedRefreshAction() => GetGroupsAsync();

        protected override void TakeIntent() {
            base.TakeIntent();

            ExecuteActionWithBusy(GetGroupsAsync);
        }

        public override Task InitializeAsync(object navigationData) {
            GroupPopup?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            GroupPopup?.Dispose();
            ResetCancellationTokenSource(ref _getGroupsCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _getGroupsCancellationTokenSource);
        }

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            _stateService.ChangedGroups += OnStateServiceChangedGroups;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            _stateService.ChangedGroups -= OnStateServiceChangedGroups;
        }

        protected override void TabbViewModelInit() {
            TabHeader = NavigationContext.GROUPS_TITLE;
            TabIcon = NavigationContext.GROUPS_IMAGE_PATH;
            RelativeViewType = typeof(GroupsView);
        }

        private Task GetGroupsAsync() =>
            Task.Run(async () => {
                ResetCancellationTokenSource(ref _getGroupsCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _getGroupsCancellationTokenSource;

                try {
                    List<GroupDTO> foundGroups = await _groupsService.GetGroupsAsync(cancellationTokenSource);

                    Device.BeginInvokeOnMainThread(() => FoundTeams = (foundGroups != null) ? new ObservableCollection<GroupDTO>(foundGroups) : new ObservableCollection<GroupDTO>());
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }
            });

        private async void OnSelectedItem(GroupInviteItemViewModel value) => await NavigationService.NavigateToAsync<GroupInfoViewModel>(value);

        private void OnStateServiceChangedGroups(object sender, ChangedGroupsSignalArgs e) => FoundTeams = new ObservableCollection<GroupDTO>(e.Data);
    }
}
