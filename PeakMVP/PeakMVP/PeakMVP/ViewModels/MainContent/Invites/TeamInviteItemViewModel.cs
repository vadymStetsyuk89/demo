using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.Responses.Invites;
using PeakMVP.Services.Invites;
using PeakMVP.ViewModels.Base;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Invites {
    public sealed class TeamInviteItemViewModel : ViewModelBase {

        private readonly IInviteService _inviteService;

        public TeamInviteItemViewModel() {
            _inviteService = ViewModelLocator.Resolve<IInviteService>();
        }

        public ICommand AcceptCommand => new Command(async () => await OnAccept(CancellationService.GetToken()));

        public ICommand DeclineCommand => new Command(async () => await OnDecline(CancellationService.GetToken()));

        string _teamName;
        public string TeamName {
            get { return _teamName; }
            set { SetProperty(ref _teamName, value); }
        }

        long _teampId;
        public long TeamId {
            get { return _teampId; }
            set { SetProperty(ref _teampId, value); }
        }

        string _owner;
        public string Owner {
            get { return _owner; }
            set { SetProperty(ref _owner, value); }
        }

        private async Task OnAccept(CancellationToken cancellationToken) {
            try {
                TeamInviteConfirmResponse teamInviteConfirmResponse = await _inviteService.TeamInviteConfirmAsync(TeamId, cancellationToken);

                if (teamInviteConfirmResponse != null) {
                    MessagingCenter.Send<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteAccepted, TeamId);

                    await DialogService.ToastAsync($"You are joined into {TeamName}!");
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

        private async Task OnDecline(CancellationToken cancellationToken) {
            try {
                TeamInviteRejectResponse teamInviteRejectResponse = await _inviteService.TeamInviteRejectAsync(TeamId, cancellationToken);

                if (teamInviteRejectResponse != null) {
                    MessagingCenter.Send<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.InviteDeclined, TeamId);

                    await DialogService.ToastAsync($"You are reject invite to {TeamName}!");
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

        public override void Dispose() {
            base.Dispose();

            CancellationService.Cancel();
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            CancellationService.Cancel();
        }
    }
}
