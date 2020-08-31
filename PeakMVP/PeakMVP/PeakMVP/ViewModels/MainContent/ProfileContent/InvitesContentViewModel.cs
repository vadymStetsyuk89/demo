using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Factories.MainContent.Invitations;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.Responses.Invites;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.Invites;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Invites;
using PeakMVP.ViewModels.MainContent.Invites.ChildrenInvites;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.ProfileContent {
    public class InvitesContentViewModel : NestedViewModel, IAskToRefresh {

        private readonly IInviteService _inviteService;
        private readonly IStateService _stateService;
        private readonly IInvitationsFactory _invitationsFactory;

        private CancellationTokenSource _getRequestsCancellationTokenSource = new CancellationTokenSource();

        public InvitesContentViewModel(
            IInviteService inviteService,
            IStateService stateService,
            IInvitationsFactory invitationsFactory) {

            _inviteService = inviteService;
            _stateService = stateService;
            _invitationsFactory = invitationsFactory;
        }

        public ICommand ViewInvitesCommand => new Command(async () => await DialogService.ToastAsync("View invites command in developing."));

        private bool _isAnyInvites;
        public bool IsAnyInvites {
            get => _isAnyInvites;
            set => SetProperty<bool>(ref _isAnyInvites, value);
        }

        private ObservableCollection<FriendshipInviteViewModel> _friendshipRequests;
        public ObservableCollection<FriendshipInviteViewModel> FriendshipRequests {
            get { return _friendshipRequests; }
            set {
                _friendshipRequests?.ForEach(fR => fR.Dispose());
                SetProperty(ref _friendshipRequests, value);

                IsAnyInvites = (FriendshipRequests != null && FriendshipRequests.Any()) ||
                    (GroupsRequests != null && GroupsRequests.Any()) ||
                    (TeamRequests != null && TeamRequests.Any()) ||
                    (ChildInvites != null && ChildInvites.Any());
            }
        }

        private ObservableCollection<GroupInviteItemViewModel> _groupsRequests;
        public ObservableCollection<GroupInviteItemViewModel> GroupsRequests {
            get { return _groupsRequests; }
            set {
                _groupsRequests?.ForEach(gR => gR.Dispose());
                SetProperty(ref _groupsRequests, value);

                IsAnyInvites = (FriendshipRequests != null && FriendshipRequests.Any()) ||
                    (GroupsRequests != null && GroupsRequests.Any()) ||
                    (TeamRequests != null && TeamRequests.Any()) ||
                    (ChildInvites != null && ChildInvites.Any());
            }
        }

        private ObservableCollection<TeamInviteItemViewModel> _teamRequests;
        public ObservableCollection<TeamInviteItemViewModel> TeamRequests {
            get { return _teamRequests; }
            set {
                _teamRequests?.ForEach(tIVM => tIVM.Dispose());
                SetProperty(ref _teamRequests, value);

                IsAnyInvites = (FriendshipRequests != null && FriendshipRequests.Any()) ||
                    (GroupsRequests != null && GroupsRequests.Any()) ||
                    (TeamRequests != null && TeamRequests.Any()) ||
                    (ChildInvites != null && ChildInvites.Any());
            }
        }

        private ObservableCollection<ChildInviteGroup> _childInvites;
        public ObservableCollection<ChildInviteGroup> ChildInvites {
            get { return _childInvites; }
            set {
                _childInvites?.ForEach(group => group.ForEach(invite => invite.Dispose()));
                SetProperty(ref _childInvites, value);

                IsAnyInvites = (FriendshipRequests != null && FriendshipRequests.Any()) ||
                    (GroupsRequests != null && GroupsRequests.Any()) ||
                    (TeamRequests != null && TeamRequests.Any()) ||
                    (ChildInvites != null && ChildInvites.Any());
            }
        }

        public Task AskToRefreshAsync() => GetRequestsAsync();

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _getRequestsCancellationTokenSource);

            FriendshipRequests?.ForEach(fR => fR.Dispose());
            GroupsRequests?.ForEach(gR => gR.Dispose());
            TeamRequests?.ForEach(tIVM => tIVM.Dispose());
            ChildInvites?.ForEach(group => group.ForEach(invite => invite.Dispose()));
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _getRequestsCancellationTokenSource);
        }

        protected override void TakeIntent() {
            base.TakeIntent();

            GetRequestsAsync();
        }

        protected override void SubscribeOnIntentEvent() {
            base.SubscribeOnIntentEvent();

            MessagingCenter.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteAccepted, (sender, teamId) => GetRequestsAsync());
            MessagingCenter.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteDeclined, (sender, teamId) => GetRequestsAsync());
            MessagingCenter.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.FriendEvents.FriendshipInviteAccepted, (sender, friendId) => GetRequestsAsync());
            MessagingCenter.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.GroupsEvents.InviteDeclined, (sender, groupId) => GetRequestsAsync());
            MessagingCenter.Subscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.GroupsEvents.InviteAccepted, (sender, groupId) => GetRequestsAsync());

            _stateService.InvitesChanged += OnStateServiceInvitesChanged;
            _stateService.ChangedFriendship += OnStateServiceChangedFriendship;
            _stateService.ChangedTeams += OnStateServiceChangedTeams;
        }

        protected override void UnsubscribeOnIntentEvent() {
            base.UnsubscribeOnIntentEvent();

            MessagingCenter.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteAccepted);
            MessagingCenter.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteDeclined);
            MessagingCenter.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.FriendEvents.FriendshipInviteAccepted);
            MessagingCenter.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.GroupsEvents.InviteDeclined);
            MessagingCenter.Unsubscribe<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.GroupsEvents.InviteAccepted);

            _stateService.InvitesChanged -= OnStateServiceInvitesChanged;
            _stateService.ChangedFriendship -= OnStateServiceChangedFriendship;
            _stateService.ChangedTeams -= OnStateServiceChangedTeams;
        }

        private Task GetRequestsAsync() =>
            Task.Run(async () => {
                ResetCancellationTokenSource(ref _getRequestsCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _getRequestsCancellationTokenSource;

                try {
                    InvitesResponse invitesResponse = await _inviteService.GetInvitesAsync(cancellationTokenSource.Token);

                    ObservableCollection<FriendshipInviteViewModel> preparedFriendshipRequests = _invitationsFactory.BuildFriendshipInvites(invitesResponse.FriendshipInvites).ToObservableCollection();
                    ObservableCollection<GroupInviteItemViewModel> preparedGroupsRequests = _invitationsFactory.BuildGroupsInvites(invitesResponse.GroupInvites).ToObservableCollection();
                    ObservableCollection<TeamInviteItemViewModel> preparedTeamRequests = _invitationsFactory.BuildTeamInvites(invitesResponse.TeamInvites).ToObservableCollection();
                    ObservableCollection<ChildInviteGroup> childInvites = _invitationsFactory.BuildChildrenInvites(invitesResponse)
                           .GroupBy<ChildInviteItemBaseViewModel, long>(childInvite => childInvite.Child.Id)
                           .Select<IGrouping<long, ChildInviteItemBaseViewModel>, ChildInviteGroup>(group => {
                               ChildInviteGroup childGroup = new ChildInviteGroup();
                               childGroup.GroupHeader = group.First().Child.DisplayName;
                               childGroup.AddRange(group);

                               return childGroup;
                           })
                           .ToObservableCollection();

                    Device.BeginInvokeOnMainThread(() => {
                        if (invitesResponse != null) {
                            FriendshipRequests = preparedFriendshipRequests;
                            GroupsRequests = preparedGroupsRequests;
                            TeamRequests = preparedTeamRequests;
                            ChildInvites = childInvites;
                        }
                        else {
                            FriendshipRequests?.Clear();
                            GroupsRequests?.Clear();
                            TeamRequests?.Clear();
                            ChildInvites?.Clear();
                        }
                    });
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }
            });

        private void OnStateServiceInvitesChanged(object sender, ChangedInvitesSignalArgs args) {
            FriendshipRequests = _invitationsFactory.BuildFriendshipInvites(args.FriendshipInvites).ToObservableCollection();
            GroupsRequests = _invitationsFactory.BuildGroupsInvites(args.GroupInvites).ToObservableCollection();
            TeamRequests = _invitationsFactory.BuildTeamInvites(args.TeamInvites).ToObservableCollection();
            ChildInvites = _invitationsFactory.BuildChildrenInvites(args)
                           .GroupBy<ChildInviteItemBaseViewModel, long>(childInvite => childInvite.Child.Id)
                           .Select<IGrouping<long, ChildInviteItemBaseViewModel>, ChildInviteGroup>(group => {
                               ChildInviteGroup childGroup = new ChildInviteGroup();
                               childGroup.GroupHeader = group.First().Child.DisplayName;
                               childGroup.AddRange(group);

                               return childGroup;
                           })
                           .ToObservableCollection();
        }

        private void OnStateServiceChangedFriendship(object sender, ChangedFriendshipSignalArgs args) => GetRequestsAsync();

        private void OnStateServiceChangedTeams(object sender, ChangedTeamsSignalArgs e) => GetRequestsAsync();
    }
}
