using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Models.Arguments.InitializeArguments.Teams;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Services.Invites;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Teams;
using PeakMVP.ViewModels.MainContent.Teams.Arguments;
using PeakMVP.Views.CompoundedViews.MainContent.Groups.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.ViewModels.MainContent.Groups.GroupPopups {
    public class AddMemberToTeamPopupViewModel : AddMemberToTheCliquePopupBaseViewModel {

        public static readonly string INVITE_MEMBER_TO_TEAM_TITLE = "Add user to team";
        private static readonly string _TEAM_INVITE_HAS_BEEN_SENT_MESSAGE = "Team invite has been sent";

        private readonly IInviteService _inviteService;

        private CancellationTokenSource _inviteMemberToTheTeamCancellationTokenSource = new CancellationTokenSource();
        private TeamDTO _targetTeam;

        public AddMemberToTeamPopupViewModel(
            IInviteService inviteService) {

            _inviteService = inviteService;
            Title = INVITE_MEMBER_TO_TEAM_TITLE;
        }

        public override Type RelativeViewType => typeof(AddMemberToGroupPopup);

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is TeamDTO) {
                _targetTeam = ((TeamDTO)navigationData);
            }
            else if (navigationData is TeamMember teamMemberDTO) {
                _targetTeam = teamMemberDTO.Team;
            }

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _inviteMemberToTheTeamCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _inviteMemberToTheTeamCancellationTokenSource);
        }

        protected async override void OnInviteExternalMemberCommand() {
            base.OnInviteExternalMemberCommand();

            await NavigationService.NavigateToAsync<InviteExternalMembersViewModel>(new InviteExternalMembersArgs() { TargetTeam = _targetTeam });
        }

        protected async override void OnAddMemberToTheClique() {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            ResetCancellationTokenSource(ref _inviteMemberToTheTeamCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _inviteMemberToTheTeamCancellationTokenSource;

            try {
                Tuple<List<ProfileDTO>, string> inviteResult = await _inviteService.AddMembersToTheTeamByTeamIdAsync(_targetTeam.Id, PossibleGroupMembers.Select<ProfileDTO, long>((pDTO) => pDTO.Id), cancellationTokenSource);

                if (string.IsNullOrEmpty(inviteResult.Item2) || string.IsNullOrWhiteSpace(inviteResult.Item2)) {
                    await DialogService.ToastAsync(AddMemberToTeamPopupViewModel._TEAM_INVITE_HAS_BEEN_SENT_MESSAGE);

                    await ((ContentPageBaseViewModel)NavigationService.LastPageViewModel).InitializeAsync(new MembersAttachedToTheTeamArgs() { AttachedMembers = inviteResult.Item1 });

                    ClosePopupCommand.Execute(null);
                }
                else {
                    await DialogService.ToastAsync(inviteResult.Item2);

                    if (inviteResult.Item1.Any()) {
                        PossibleGroupMembers = PossibleGroupMembers
                                        .Except<ProfileDTO>(PossibleGroupMembers.Where<ProfileDTO>((pDTO) => inviteResult.Item1.Any(invitedPDTO => invitedPDTO.Id == pDTO.Id)))
                                        .ToObservableCollection();

                        await ((ContentPageBaseViewModel)NavigationService.LastPageViewModel).InitializeAsync(new MembersAttachedToTheTeamArgs() { AttachedMembers = inviteResult.Item1 });
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                await DialogService.ToastAsync(exc.Message);
            }

            UpdateBusyVisualState(busyKey, false);
        }
    }
}
