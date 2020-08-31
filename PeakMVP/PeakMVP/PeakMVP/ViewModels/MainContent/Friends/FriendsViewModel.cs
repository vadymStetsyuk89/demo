using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Factories.MainContent;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.Friends;
using PeakMVP.Services.Invites;
using PeakMVP.Services.Navigation;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Invites;
using PeakMVP.Views.CompoundedViews.MainContent.Friends;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.Friends {
    public sealed class FriendsViewModel : TabbedViewModel {

        private readonly IFriendFactory _friendFactory;
        private readonly IFriendService _friendService;
        private readonly IStateService _stateService;

        private CancellationTokenSource _getFriendsCancellationTokenSource = new CancellationTokenSource();

        public FriendsViewModel(
            IFriendService friendService,
            IInviteService inviteService,
            IFriendFactory friendFactory,
            INavigationContext bottomBarDataItems,
            IStateService stateService) {

            _friendFactory = friendFactory;
            _friendService = friendService;
            _stateService = stateService;

            IsNestedPullToRefreshEnabled = true;
        }

        ObservableCollection<FriendshipInviteViewModel> _friends;
        public ObservableCollection<FriendshipInviteViewModel> Friends {
            get { return _friends; }
            set {
                if (_friends != null) {
                    _friends.ForEach(fIVM => fIVM.Dispose());
                }

                SetProperty(ref _friends, value);
            }
        }

        FriendshipInviteViewModel _selectedItem;
        public FriendshipInviteViewModel SelectedItem {
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

        protected override Task NestedRefreshAction() => GetFriendsAsync();

        protected override void TakeIntent() {
            base.TakeIntent();

            ExecuteActionWithBusy(GetFriendsAsync);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _getFriendsCancellationTokenSource);

            Friends?.ForEach(f => f.Dispose());
            Friends?.Clear();
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _getFriendsCancellationTokenSource);
        }

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            MessagingCenter.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.FriendEvents.FriendDeleted, (sender, args) => ExecuteActionWithBusy(GetFriendsAsync));
            _stateService.ChangedFriendship += OnStateServiceChangedFriendship;
            _stateService.InvitesChanged += OnStateServiceInvitesChanged;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            MessagingCenter.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.FriendEvents.FriendDeleted);
            _stateService.ChangedFriendship -= OnStateServiceChangedFriendship;
            _stateService.InvitesChanged -= OnStateServiceInvitesChanged;
        }

        protected override void TabbViewModelInit() {
            TabHeader = NavigationContext.FRIENDS_TITLE;
            TabIcon = NavigationContext.FRIENDS_IMAGE_PATH;
            RelativeViewType = typeof(FriendsView);
        }

        private async void OnSelectedItem(FriendshipInviteViewModel value) => await NavigationService.NavigateToAsync<ProfileInfoViewModel>(value);

        private Task GetFriendsAsync() =>
            Task.Run(async () => {
                ResetCancellationTokenSource(ref _getFriendsCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _getFriendsCancellationTokenSource;

                try {
                    List<FriendshipDTO> friends = await _friendService.GetAllFriendsAsync(cancellationTokenSource.Token);
                    ObservableCollection<FriendshipInviteViewModel> resolvedFriends = (friends != null && friends.Any()) ? (_friendFactory.CreateFriends(friends)).ToObservableCollection() : new ObservableCollection<FriendshipInviteViewModel>();

                    Device.BeginInvokeOnMainThread(() => Friends = resolvedFriends);
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }
            });

        private void OnStateServiceChangedFriendship(object sender, ChangedFriendshipSignalArgs args) =>
            Friends = (args.Data != null) ? (_friendFactory.CreateFriends(args.Data)).ToObservableCollection() : new ObservableCollection<FriendshipInviteViewModel>();

        private void OnStateServiceInvitesChanged(object sender, ChangedInvitesSignalArgs e) => ExecuteActionWithBusy(GetFriendsAsync);
    }
}
