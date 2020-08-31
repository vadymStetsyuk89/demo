using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.Arguments.InitializeArguments;
using PeakMVP.Models.Arguments.InitializeArguments.Teams;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Teams;
using PeakMVP.Models.Rests.Responses.Teams;
using PeakMVP.Services.Teams;
using PeakMVP.ViewModels.Base;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.ViewModels.MainContent.ActionBars.Top {
    public class ExternalInviteActionBarViewModel : ExecutionActionBarBaseViewModel {

        public static readonly string INVITE_EXTERNAL_MEMBER_TO_TEAM_TITLE = "Invite to";
        public static readonly string EXTERNAL_INVITE_WAS_SENT = "External invite was sent";

        private readonly ITeamService _teamService;

        private CancellationTokenSource _inviteExternalMemberToTeamAsyncCancellationTokenSource = new CancellationTokenSource();

        public ExternalInviteActionBarViewModel(
            ITeamService teamService) {

            _teamService = teamService;
        }

        public override Task InitializeAsync(object navigationData) {

            if (navigationData is InviteExternalMembersArgs inviteExternalMembersArgs) {
                Title = string.Format("{0} {1}", INVITE_EXTERNAL_MEMBER_TO_TEAM_TITLE, inviteExternalMembersArgs.TargetTeam?.Name);
            }

            return base.InitializeAsync(navigationData);
        }

        public override void ResolveExecutionAvailability(object condition) {
            base.ResolveExecutionAvailability(condition);

            if (condition is bool boolCondition) {
                IsEcutionAvailable = boolCondition;
            }
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _inviteExternalMemberToTeamAsyncCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _inviteExternalMemberToTeamAsyncCancellationTokenSource);
        }

        protected async override void OnExecuteCommand() {
            base.OnExecuteCommand();

            if (NavigationService.LastPageViewModel is IInputForm inputForm) {
                if (inputForm.ValidateForm()) {
                    if (inputForm is IBuildFormModel buildFormModel) {
                        Guid busyKey = Guid.NewGuid();
                        UpdateBusyVisualState(busyKey, true);

                        ResetCancellationTokenSource(ref _inviteExternalMemberToTeamAsyncCancellationTokenSource);
                        CancellationTokenSource cancellationTokenSource = _inviteExternalMemberToTeamAsyncCancellationTokenSource;

                        try {
                            object formModel = buildFormModel.BuildFormModel();
                            InviteExternalMemberToTeamResponse inviteExternalMemberToTeamResponse = await _teamService.InviteExternalMemberToTeamAsync(formModel as ExternalMemberTeamIntive, cancellationTokenSource);

                            if (inviteExternalMemberToTeamResponse != null) {
                                await DialogService.ToastAsync(EXTERNAL_INVITE_WAS_SENT);
                                await NavigationService.GoBackAsync(new IntentViewModelArgs());
                            }
                            else {
                                await DialogService.ToastAsync(TeamService.INVITE_EXTERNAL_MEMBER_COMMON_ERROR_MESSAGE);
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
        }
    }
}
