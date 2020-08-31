using Microsoft.AppCenter.Crashes;
using PeakMVP.Helpers;
using PeakMVP.Models.AppNavigation.Character;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Models.Rests.Responses.Friends;
using PeakMVP.Models.Sockets.StateArgs;
using PeakMVP.Services.Friends;
using PeakMVP.Services.Profile;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.Services.TeamMembers;
using PeakMVP.Services.Teams;
using PeakMVP.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Character {
    public abstract class CharacterInfoViewModelBase : LoggedContentPageViewModel {

        protected readonly ITeamMemberService _teamMemberService;
        private readonly IFriendService _friendService;
        private readonly IStateService _stateService;
        private readonly IProfileService _profileService;
        private readonly ITeamService _teamService;

        public CharacterInfoViewModelBase(
            IProfileService profileService,
            IStateService stateService,
            ITeamService teamService,
            IFriendService friendService,
            ITeamMemberService teamMemberService) {

            _profileService = profileService;
            _stateService = stateService;
            _teamService = teamService;
            _friendService = friendService;
            _teamMemberService = teamMemberService;

            RefreshCommand = new Command(async () => {
                IsRefreshing = true;

                await ResolveCharacterProfileAsync();
                await ResolveTargetTeamAsync();
                await ResolveIsFriendltAsync(TargetMember.Id);
                await ResolveContactInfosAsync();

                await Task.Delay(AppConsts.DELAY_STUB);
                IsRefreshing = false;
            });
        }

        private CancellationTokenSource _resolveCharacterProfileCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _resolveTargetTeamCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _isFriendlyCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _resolveContactInfosCancellationTokenSource = new CancellationTokenSource();

        private ProfileDTO _targetMember;
        public ProfileDTO TargetMember {
            get => _targetMember;
            private set => SetProperty<ProfileDTO>(ref _targetMember, value);
        }

        private TeamDTO _targetTeam;
        public TeamDTO TargetTeam {
            get => _targetTeam;
            private set => SetProperty<TeamDTO>(ref _targetTeam, value);
        }

        private TeamMember _relativeTeamMember;
        public TeamMember RelativeTeamMember {
            get => _relativeTeamMember;
            private set => SetProperty<TeamMember>(ref _relativeTeamMember, value);
        }

        private List<TeamMemberContactInfo> _contactInfos;
        public List<TeamMemberContactInfo> ContactInfos {
            get => _contactInfos;
            private set => SetProperty<List<TeamMemberContactInfo>>(ref _contactInfos, value);
        }

        private bool _isFriend;
        public bool IsFriend {
            get => _isFriend;
            private set => SetProperty<bool>(ref _isFriend, value);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _resolveCharacterProfileCancellationTokenSource);
            ResetCancellationTokenSource(ref _resolveTargetTeamCancellationTokenSource);
            ResetCancellationTokenSource(ref _isFriendlyCancellationTokenSource);
            ResetCancellationTokenSource(ref _resolveContactInfosCancellationTokenSource);
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is ViewTeamMemberCharacterInfoArgs characterInfoArgs) {
                TargetTeam = characterInfoArgs.TargetTeam;
                TargetMember = characterInfoArgs.TargetMember;
                RelativeTeamMember = characterInfoArgs.RelativeTeamMember;

                ExecuteActionWithBusy(ResolveCharacterProfileAsync);
                ExecuteActionWithBusy(ResolveTargetTeamAsync);
                ResolveIsFriendltAsync(TargetMember.Id);
                ResolveContactInfosAsync();
            }

            return base.InitializeAsync(navigationData);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _resolveCharacterProfileCancellationTokenSource);
            ResetCancellationTokenSource(ref _resolveTargetTeamCancellationTokenSource);
            ResetCancellationTokenSource(ref _isFriendlyCancellationTokenSource);
            ResetCancellationTokenSource(ref _resolveContactInfosCancellationTokenSource);
        }

        protected override void OnSubscribeOnAppEvents() {
            base.OnSubscribeOnAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.OuterProfileInfoUpdated += OnProfileSettingsEventsOuterProfileInfoUpdated;
            _stateService.ChangedProfile += OnstateServiceChangedProfile;
            _stateService.ChangedFriendship += OnStateServiceChangedFriendship;
            _stateService.InvitesChanged += OnStateServiceInvitesChanged;
            GlobalSettings.Instance.AppMessagingEvents.TeamMemberEvents.AddedTeamMemberContactInfo += OnTeamMemberEventsAddedTeamMemberContactInfo;
            GlobalSettings.Instance.AppMessagingEvents.TeamMemberEvents.DeletedTeamMemberContactInfo += OnTeamMemberEventsDeletedTeamMemberContactInfo;
        }

        protected override void OnUnsubscribeFromAppEvents() {
            base.OnUnsubscribeFromAppEvents();

            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.OuterProfileInfoUpdated -= OnProfileSettingsEventsOuterProfileInfoUpdated;
            _stateService.ChangedProfile -= OnstateServiceChangedProfile;
            _stateService.ChangedFriendship -= OnStateServiceChangedFriendship;
            _stateService.InvitesChanged -= OnStateServiceInvitesChanged;
            GlobalSettings.Instance.AppMessagingEvents.TeamMemberEvents.AddedTeamMemberContactInfo -= OnTeamMemberEventsAddedTeamMemberContactInfo;
            GlobalSettings.Instance.AppMessagingEvents.TeamMemberEvents.DeletedTeamMemberContactInfo -= OnTeamMemberEventsDeletedTeamMemberContactInfo;
        }

        protected virtual void OnResolveCharacterProfile() { }

        protected virtual void OnResolveTargetTeam() { }

        private Task ResolveContactInfosAsync() =>
            Task.Run(async () => {
                List<TeamMemberContactInfo> resolvedContactInfos = null;
                TeamMember resolvedTeamMember = null;

                if (RelativeTeamMember == null) {
                    Device.BeginInvokeOnMainThread(() => {
                        ContactInfos = resolvedContactInfos;
                        RelativeTeamMember = resolvedTeamMember;
                    });

                    return;
                }

                ResetCancellationTokenSource(ref _resolveContactInfosCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _resolveContactInfosCancellationTokenSource;

                try {
                    resolvedTeamMember = await _teamMemberService.GetTeamMemberByMemberIdAsync(TargetMember.Id, cancellationTokenSource.Token);
                    if (resolvedTeamMember != null) {

                        resolvedContactInfos = resolvedTeamMember.ContactInfo;
                    }
                    else {
                        resolvedTeamMember = null;
                        resolvedContactInfos = null;
                    }

                    //List<TeamMember> teamMembers = await _teamMemberService.GetTeamMemberByMemberIdAsync(TargetMember.Id, cancellationTokenSource.Token);

                    //if (teamMembers != null) {
                    //    resolvedTeamMember = teamMembers
                    //        .FirstOrDefault<TeamMember>(foundTeamMember => foundTeamMember.Id == RelativeTeamMember.Id &&
                    //            foundTeamMember.Member?.Id == TargetMember.Id &&
                    //            foundTeamMember.Team?.Id == TargetTeam.Id);

                    //    resolvedContactInfos = resolvedTeamMember != null ? resolvedTeamMember.ContactInfo : null;
                    //}
                    //else {
                    //    resolvedTeamMember = null;
                    //    resolvedContactInfos = null;
                    //}
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    resolvedTeamMember = null;
                    resolvedContactInfos = null;

                    await DialogService.ToastAsync(exc.Message);
                }

                Device.BeginInvokeOnMainThread(() => {
                    ContactInfos = resolvedContactInfos;
                    RelativeTeamMember = resolvedTeamMember;
                });
            });

        private Task ResolveCharacterProfileAsync() =>
            Task.Run(async () => {
                if (_targetMember == null) {
                    return;
                }

                ResetCancellationTokenSource(ref _resolveCharacterProfileCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _resolveCharacterProfileCancellationTokenSource;

                try {
                    ProfileDTO profile = await _profileService.GetProfileByIdAsync(_targetMember.Id, cancellationTokenSource.Token);
                    if (profile != null) {
                        _targetMember = profile;

                        Device.BeginInvokeOnMainThread(() => {
                            OnResolveCharacterProfile();
                        });
                    }
                    else {
                        throw new InvalidOperationException(ProfileService.CANT_RESOLVE_PROFILE_COMMON_ERROR_MESSAGE);
                    }
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }
            });

        private Task ResolveTargetTeamAsync() =>
            Task.Run(async () => {
                if (_targetTeam == null) {
                    return;
                }

                ResetCancellationTokenSource(ref _resolveTargetTeamCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _resolveTargetTeamCancellationTokenSource;

                try {
                    TeamDTO team = await _teamService.GetTeamByIdAsync(_targetTeam.Id, cancellationTokenSource);

                    if (team != null) {
                        _targetTeam = team;

                        Device.BeginInvokeOnMainThread(() => {
                            OnResolveTargetTeam();
                        });
                    }
                    else {
                        throw new InvalidOperationException(TeamService.CANT_GET_TEAM_ERROR);
                    }
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }
            });

        private Task ResolveIsFriendltAsync(long profileId) =>
            Task.Run(async () => {

                ResetCancellationTokenSource(ref _isFriendlyCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _isFriendlyCancellationTokenSource;

                try {
                    bool isFriend = false;
                    GetFriendByIdResponse friend = await _friendService.GetFriendByIdAsync(profileId, cancellationTokenSource.Token);

                    if (friend != null && friend.Profile != null) {
                        isFriend = true;
                    }
                    else {
                        isFriend = false;
                    }

                    Device.BeginInvokeOnMainThread(() => {
                        IsFriend = isFriend;
                    });
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    Device.BeginInvokeOnMainThread(() => {
                        IsFriend = false;
                    });
                }
            });

        private async void OnstateServiceChangedProfile(object sender, ChangedProfileSignalArgs e) {
            await ResolveCharacterProfileAsync();
            await ResolveContactInfosAsync();
        }

        private void OnStateServiceInvitesChanged(object sender, ChangedInvitesSignalArgs e) {
            try {
                ResolveIsFriendltAsync(_targetMember.Id);
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);
            }
        }

        private void OnStateServiceChangedFriendship(object sender, ChangedFriendshipSignalArgs e) {
            try {
                ResolveIsFriendltAsync(_targetMember.Id);
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);
            }
        }

        private async void OnProfileSettingsEventsOuterProfileInfoUpdated(object sender, EventArgs e) {
            await ResolveCharacterProfileAsync();
            await ResolveTargetTeamAsync();
            await ResolveContactInfosAsync();
        }

        private async void OnTeamMemberEventsAddedTeamMemberContactInfo(object sender, EventArgs e) {
            await ResolveCharacterProfileAsync();
            await ResolveTargetTeamAsync();
            await ResolveContactInfosAsync();
        }

        private async void OnTeamMemberEventsDeletedTeamMemberContactInfo(object sender, EventArgs e) {
            Guid busyKey = Guid.NewGuid();
            SetBusy(busyKey, true);

            try {
                await ResolveCharacterProfileAsync();
                await ResolveTargetTeamAsync();
                await ResolveContactInfosAsync();
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);
            }

            SetBusy(busyKey, false);
        }
    }
}
