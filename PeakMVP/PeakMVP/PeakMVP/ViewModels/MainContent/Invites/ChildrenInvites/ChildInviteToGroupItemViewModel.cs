using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Groups;
using PeakMVP.Models.Rests.Responses.Groups;
using PeakMVP.Services.Groups;
using System;
using System.Threading;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Invites.ChildrenInvites {
    public class ChildInviteToGroupItemViewModel : ChildInviteItemBaseViewModel {

        private readonly IGroupsService _groupsService;

        public ChildInviteToGroupItemViewModel(
            IGroupsService groupsService) {

            _groupsService = groupsService;

            AcceptCommand = new Command(async () => {
                try {
                    GroupRequestConfirmDataModel groupRequestConfirmDataModel = new GroupRequestConfirmDataModel {
                        GroupId = ((GroupDTO)InviteTo).Id,
                        Id = Child.Id,
                    };

                    GroupRequestConfirmResponse groupRequestConfirmResponse =
                        await _groupsService.GroupRequestConfirmAsync(groupRequestConfirmDataModel, default(CancellationToken));

                    if (groupRequestConfirmResponse != null) {
                        MessagingCenter.Send<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.GroupsEvents.InviteAccepted, ((GroupDTO)InviteTo).Id);

                        await DialogService.ToastAsync("Child joined to group");
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
                    GroupRequestDeclineDataModel dataModel = new GroupRequestDeclineDataModel {
                        GroupId = ((GroupDTO)InviteTo).Id,
                        Id = Child.Id
                    };

                    if (await _groupsService.GroupRequestDeclineAsync(dataModel, default(CancellationToken))) {
                        MessagingCenter.Send<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.GroupsEvents.InviteDeclined, ((GroupDTO)InviteTo).Id);

                        await DialogService.ToastAsync("Child declined invite to group");
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
            CompanionScopeTitle = (InviteTo is GroupDTO group) ? group.Name : "";
            CompanionAvatarPath = null;
            MainInviteDescription = null;
        }
    }
}
