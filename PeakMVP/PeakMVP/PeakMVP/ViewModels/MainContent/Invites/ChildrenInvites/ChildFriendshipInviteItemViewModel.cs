using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.Friends;
using PeakMVP.Services.Friends;
using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Invites.ChildrenInvites {
    public class ChildFriendshipInviteItemViewModel : ChildInviteItemBaseViewModel {

        private readonly IFriendService _friendService;

        public ChildFriendshipInviteItemViewModel(
            IFriendService friendService) {

            _friendService = friendService;

            IsAvatarEnabled = true;

            AcceptCommand = new Command(async () => {
                try {
                    ConfirmFriendResponse confirmFriendResponse = await _friendService.RequestConfirmAsync(((ProfileDTO)InviteTo).Id, childProfileId: Child.Id);

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

                    await DialogService.ToastAsync(exc.Message);
                }
            });

            DeclineCommand = new Command(async () => {
                try {
                    DeleteFriendResponse deleteFriendResponse = await _friendService.RequestDeleteAsync(((ProfileDTO)InviteTo).Id, childProfileId: Child.Id);

                    if (deleteFriendResponse != null) {
                        MessagingCenter.Instance.Send<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.FriendEvents.FriendshipInviteDeclined, ((ProfileDTO)InviteTo).Id);

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
            });
        }

        protected override void OnUploadParticipants() {
            if (InviteTo is ProfileDTO friend) {
                CompanionScopeTitle = friend.DisplayName;
                CompanionAvatarPath = (friend.Avatar != null) ? friend.Avatar.Url : "";
                MainInviteDescription = friend.Type;
            }
            else {
                CompanionScopeTitle = null;
                CompanionAvatarPath = null;
                MainInviteDescription = null;
            }
        }
    }
}
