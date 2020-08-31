using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Teams;
using PeakMVP.Services.Teams;
using PeakMVP.ViewModels.Base;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Invites {
    public sealed class TeamRequestItemViewModel : ViewModelBase {

        private readonly ITeamService _teamService;

        public TeamRequestItemViewModel() {
            _teamService = ViewModelLocator.Resolve<ITeamService>();
        }

        public ICommand AcceptCommand => new Command(async () => await OnAccept(CancellationService.GetToken()));

        public ICommand DeclineCommand => new Command(async () => await OnDecline(CancellationService.GetToken()));

        ProfileDTO _profile;
        public ProfileDTO Profile {
            get { return _profile; }
            set { SetProperty(ref _profile, value); }
        }

        TeamDTO _team;
        public TeamDTO Team {
            get { return _team; }
            set { SetProperty(ref _team, value); }
        }

        long _id;
        public long Id {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private async Task OnAccept(CancellationToken cancellationToken) =>
            await Task.Run(async () => {
                try {
                    ApproveTeamRequestsResponse approveTeamRequestsResponse =
                        await _teamService.ApproveTeamRequestsAsync(Id, Team.Id, cancellationToken);

                    if (approveTeamRequestsResponse != null) {
                        MessagingCenter.Send<object>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.RequestAcceptedForNewTeamMember);
                    }
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    Debug.WriteLine($"ERROR:{exc.Message}");
                }
            });

        private async Task OnDecline(CancellationToken cancellationToken) =>
            await Task.Run(async () => {
                try {
                    RejectTeamRequestsResponse rejectTeamRequestsResponse =
                        await _teamService.RejectTeamRequestsAsync(Id, Team.Id, cancellationToken);

                    if (rejectTeamRequestsResponse != null) {
                        MessagingCenter.Send<object>(this, GlobalSettings.Instance.AppMessagingEvents.TeamEvents.RequestDeclinedForNewTeamMember);
                    }
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    Debug.WriteLine($"ERROR:{exc.Message}");
                }
            });
    }
}
