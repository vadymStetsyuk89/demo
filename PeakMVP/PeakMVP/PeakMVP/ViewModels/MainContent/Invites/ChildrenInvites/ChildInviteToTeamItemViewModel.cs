using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Invites;
using PeakMVP.Services.Invites;
using System;
using System.Threading;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Invites.ChildrenInvites {
    public class ChildInviteToTeamItemViewModel : ChildInviteItemBaseViewModel {

        private readonly IInviteService _inviteService;

        public ChildInviteToTeamItemViewModel(IInviteService inviteService) {

            _inviteService = inviteService;

            AcceptCommand = new Command(async () => {
                try {
                    TeamInviteConfirmResponse teamInviteConfirmResponse = await _inviteService.TeamInviteConfirmAsync(((TeamDTO)InviteTo).Id, default(CancellationToken), Child.Id);

                    if (teamInviteConfirmResponse != null) {
                        MessagingCenter.Send<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteAccepted, ((TeamDTO)InviteTo).Id);

                        await DialogService.ToastAsync("Child joined to team");
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

            DeclineCommand = new Command(async () => {
                try {
                    TeamInviteRejectResponse teamInviteRejectResponse = await _inviteService.TeamInviteRejectAsync(((TeamDTO)InviteTo).Id, default(CancellationToken), Child.Id);

                    if (teamInviteRejectResponse != null) {
                        MessagingCenter.Send<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteDeclined, ((TeamDTO)InviteTo).Id);

                        await DialogService.ToastAsync("Child reject invite to team");
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
        }

        protected override void OnUploadParticipants() {
            if (InviteTo is TeamDTO team) {
                CompanionScopeTitle = team.Name;
                MainInviteDescription = string.Format("from: {0} to: {1}", team.Name, Child.DisplayName);
            }
            else {
                CompanionScopeTitle = null;
                MainInviteDescription = null;
            }
        }
    }
}
