using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Groups;
using PeakMVP.Models.Rests.Responses.Groups;
using PeakMVP.Services.Groups;
using PeakMVP.ViewModels.Base;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Invites {
    public sealed class GroupInviteItemViewModel : ViewModelBase {

        private readonly IGroupsService _groupsService;

        string _groupName;
        public string GroupName {
            get { return _groupName; }
            set { SetProperty(ref _groupName, value); }
        }

        long _groupId;
        public long GroupId {
            get { return _groupId; }
            set { SetProperty(ref _groupId, value); }
        }

        public ICommand AcceptCommand => new Command(async () => await OnAccept(CancellationService.GetToken()));

        public ICommand DeclineCommand => new Command(async () => await OnDecline(CancellationService.GetToken()));

        public GroupInviteItemViewModel() {
            _groupsService = ViewModelLocator.Resolve<IGroupsService>();
        }

        public override void Dispose() {
            base.Dispose();

            CancellationService.Cancel();
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            CancellationService.Cancel();
        }

        private async Task OnAccept(CancellationToken cancellationToken) {
            try {
                GroupRequestConfirmDataModel groupRequestConfirmDataModel = new GroupRequestConfirmDataModel {
                    GroupId = this.GroupId,
                    Id = GlobalSettings.Instance.UserProfile.Id
                };

                GroupRequestConfirmResponse groupRequestConfirmResponse =
                    await _groupsService.GroupRequestConfirmAsync(groupRequestConfirmDataModel, cancellationToken);

                if (groupRequestConfirmResponse != null) {
                    MessagingCenter.Send<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.GroupsEvents.InviteAccepted, GroupId);

                    await DialogService.ToastAsync($"You are joined in {GroupName}!");
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
                GroupRequestDeclineDataModel dataModel = new GroupRequestDeclineDataModel {
                    GroupId = this.GroupId,
                    Id = GlobalSettings.Instance.UserProfile.Id
                };

                if (await _groupsService.GroupRequestDeclineAsync(dataModel, cancellationToken)) {
                    MessagingCenter.Send<object, long>(this, GlobalSettings.Instance.AppMessagingEvents.GroupsEvents.InviteDeclined, GroupId);

                    await DialogService.ToastAsync($"You are declined invite to {GroupName}!");
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
    }
}
