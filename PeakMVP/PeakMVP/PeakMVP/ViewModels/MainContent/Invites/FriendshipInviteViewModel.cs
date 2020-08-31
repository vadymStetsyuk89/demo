using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Friends;
using PeakMVP.Services.Friends;
using PeakMVP.ViewModels.Base;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Invites {
    public sealed class FriendshipInviteViewModel : ViewModelBase {

        private readonly IFriendService _friendService;

        private ProfileDTO _profile;
        public ProfileDTO Profile {
            get => _profile;
            set => SetProperty<ProfileDTO>(ref _profile, value);
        }

        string _avatar;
        public string Avatar {
            get => _avatar;
            set { SetProperty(ref _avatar, value); }
        }

        string _fullName;
        public string FullName {
            get => _fullName;
            set { SetProperty(ref _fullName, value); }
        }

        long _friendId;
        public long FriendId {
            get => _friendId;
            set { SetProperty(ref _friendId, value); }
        }

        string _profileType;
        public string ProfileType {
            get { return _profileType; }
            set { SetProperty(ref _profileType, value); }
        }

        bool _isRequest;
        public bool IsRequest {
            get => _isRequest;
            set { SetProperty(ref _isRequest, value); }
        }

        public ICommand AcceptCommand => new Command(async () => await OnAccept(CancellationService.GetToken()));

        public ICommand DeclineCommand => new Command(async () => await OnDecline(CancellationService.GetToken()));

        public FriendshipInviteViewModel() {
            _friendService = ViewModelLocator.Resolve<IFriendService>();
        }

        private async Task OnDecline(CancellationToken cancellationToken) {
            try {
                DeleteFriendResponse deleteFriendResponse = await _friendService.RequestDeleteAsync(FriendId, cancellationToken);

                if (deleteFriendResponse != null) {
                    MessagingCenter.Instance.Send<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.FriendEvents.FriendshipInviteDeclined, FriendId);

                    await DialogService.ToastAsync("Friend request rejected!");
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                Debug.WriteLine($"ERROR:{exc.Message}");
                await DialogService.ToastAsync(exc.Message);
            }
        }

        private async Task OnAccept(CancellationToken cancellationToken) {
            try {
                ConfirmFriendResponse confirmFriendResponse = await _friendService.RequestConfirmAsync(FriendId, cancellationToken);

                if (confirmFriendResponse != null) {
                    MessagingCenter.Send<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.FriendEvents.FriendshipInviteAccepted, confirmFriendResponse.Id);

                    await DialogService.ToastAsync("Friend added!");
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                Debug.WriteLine($"ERROR:{exc.Message}");
                Debugger.Break();
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
