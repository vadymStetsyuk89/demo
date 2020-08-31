using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Requests.RequestDataModels.Groups;
using PeakMVP.Services.Groups;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Groups.Arguments;
using PeakMVP.Views.CompoundedViews.MainContent.Groups.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PeakMVP.ViewModels.MainContent.Groups.GroupPopups {
    public class AddMemberToGroupPopupViewModel : AddMemberToTheCliquePopupBaseViewModel {

        public static readonly string INVITE_MEMBER_TO_GROUP_TITLE = "Add user to group";
        private static readonly string _GROUP_INVITE_HAS_BEEN_SENT_MESSAGE = "Group invite has been sent";

        private readonly IGroupsService _groupsService;

        private CancellationTokenSource _attachMembersToTheGroupCancellationTokenSource = new CancellationTokenSource();
        private GroupDTO _targetGroup;

        public AddMemberToGroupPopupViewModel(
            IGroupsService groupsService) {

            _groupsService = groupsService;
            Title = INVITE_MEMBER_TO_GROUP_TITLE;
        }

        public override Type RelativeViewType => typeof(AddMemberToGroupPopup);

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _attachMembersToTheGroupCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _attachMembersToTheGroupCancellationTokenSource);
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is GroupDTO) {
                _targetGroup = ((GroupDTO)navigationData);
            }

            return base.InitializeAsync(navigationData);
        }

        protected async override void OnAddMemberToTheClique() {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            ResetCancellationTokenSource(ref _attachMembersToTheGroupCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _attachMembersToTheGroupCancellationTokenSource;

            try {
                List<MemberDTO> attachedMembers = await _groupsService.InviteMemberToTheGroupAsync(new InviteMembersToTheGroupDataModel() {
                    ProfileId = GlobalSettings.Instance.UserProfile.Id,
                    GroupId = _targetGroup.Id,
                    MembersIds = PossibleGroupMembers.Select<ProfileDTO, long>(pDTO => pDTO.Id).ToArray<long>()
                }, cancellationTokenSource);

                if (attachedMembers != null && attachedMembers.Any()) {
                    await DialogService.ToastAsync(_GROUP_INVITE_HAS_BEEN_SENT_MESSAGE);
                    ///
                    /// TODO: use appropriate messaging event (remove old approach)
                    /// 
                    await ((ContentPageBaseViewModel)NavigationService.LastPageViewModel).InitializeAsync(new MembersAttachedToTheGroupArgs() { AttachedMembers = attachedMembers });

                    ClosePopupCommand.Execute(null);
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
