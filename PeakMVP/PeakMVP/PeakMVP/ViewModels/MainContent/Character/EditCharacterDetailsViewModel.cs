using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.Arguments.InitializeArguments.Profile;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Services.Friends;
using PeakMVP.Services.Profile;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.Services.TeamMembers;
using PeakMVP.Services.Teams;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Character.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Character {
    public class EditCharacterDetailsViewModel : CharacterInfoViewModelBase {

        public static readonly string ONLY_PARENT_PERMISSION_CONTACT_INFO_WARNING = "Only parent can manage contact infos";
        public static readonly string ONLY_TEAM_MANAGER_PERMISSION_PROFILE_EDIT_WARNING = "Only team manager (coach/organization) can edit team member info";

        private CancellationTokenSource _removeContactInfoCancellationTokenSource = new CancellationTokenSource();

        public EditCharacterDetailsViewModel(
            IProfileService profileService,
            IStateService stateService,
            ITeamService teamService,
            IFriendService friendService,
            ITeamMemberService teamMemberService)
            : base(
                  profileService,
                  stateService,
                  teamService,
                  friendService,
                  teamMemberService) {

            EditOuterCharacterPopupViewModel = ViewModelLocator.Resolve<EditOuterCharacterPopupViewModel>();
            EditOuterCharacterPopupViewModel?.InitializeAsync(this);

            AddTeamMemberContactNotePopupViewModel = ViewModelLocator.Resolve<AddTeamMemberContactNotePopupViewModel>();
            AddTeamMemberContactNotePopupViewModel?.InitializeAsync(this);
        }

        public ICommand EditCharacterCommand => new Command(async () => {
            if (EditPermission) {
                EditOuterCharacterPopupViewModel?.ShowPopupCommand.Execute(new EditOuterProfileArgs() {
                    RelatedTeamId = TargetTeam.Id,
                    TargetProfile = TargetMember
                });
            }
            else {
                await DialogService.ToastAsync(ONLY_TEAM_MANAGER_PERMISSION_PROFILE_EDIT_WARNING);
            }
        });

        public ICommand AddPhoneCommand => new Command(async () => {
            if (AddContactInfoPermission) {
                AddTeamMemberContactNotePopupViewModel.ShowPopupCommand.Execute(RelativeTeamMember);
            }
            else {
                await DialogService.ToastAsync(ONLY_PARENT_PERMISSION_CONTACT_INFO_WARNING);
            }
        });

        public ICommand RemoveContactInfoCommand => new Command(async (object param) => {
            if (param is TeamMemberContactInfo contactInfo) {
                if (AddContactInfoPermission) {
                    Guid busyKey = Guid.NewGuid();
                    SetBusy(busyKey, true);

                    try {
                        ResetCancellationTokenSource(ref _removeContactInfoCancellationTokenSource);
                        CancellationTokenSource cancellationTokenSource = _removeContactInfoCancellationTokenSource;

                        TeamMemberContactInfo[] modifiedSequence = ContactInfos.Except<TeamMemberContactInfo>(new TeamMemberContactInfo[] { contactInfo }).ToArray<TeamMemberContactInfo>();

                        if (await _teamMemberService.EditContactInfoAsync(RelativeTeamMember.Id, modifiedSequence, cancellationTokenSource)) {
                            GlobalSettings.Instance.AppMessagingEvents.TeamMemberEvents.DeletedTeamMemberContactInfoInvoke(this, new EventArgs());
                        }
                        else {
                            throw new InvalidOperationException(TeamMemberService.CANT_DELETE_CONTACT_INFO_COMMON_WARNING);
                        }
                    }
                    catch (OperationCanceledException) { }
                    catch (ObjectDisposedException) { }
                    catch (ServiceAuthenticationException) { }
                    catch (Exception exc) {
                        Crashes.TrackError(exc);

                        await DialogService.ToastAsync(exc.Message);
                    }

                    SetBusy(busyKey, false);
                }
                else {
                    await DialogService.ToastAsync(ONLY_PARENT_PERMISSION_CONTACT_INFO_WARNING);
                }
            }
        });

        private EditOuterCharacterPopupViewModel _editOuterCharacterPopupViewModel;
        public EditOuterCharacterPopupViewModel EditOuterCharacterPopupViewModel {
            get => _editOuterCharacterPopupViewModel;
            private set => SetProperty<EditOuterCharacterPopupViewModel>(ref _editOuterCharacterPopupViewModel, value);
        }

        private AddTeamMemberContactNotePopupViewModel _addTeamMemberContactNotePopupViewModel;
        public AddTeamMemberContactNotePopupViewModel AddTeamMemberContactNotePopupViewModel {
            get => _addTeamMemberContactNotePopupViewModel;
            private set => SetProperty<AddTeamMemberContactNotePopupViewModel>(ref _addTeamMemberContactNotePopupViewModel, value);
        }

        private bool _editPermission;
        public bool EditPermission {
            get => _editPermission;
            protected set => SetProperty<bool>(ref _editPermission, value);
        }

        private bool _addContactInfoPermission;
        public bool AddContactInfoPermission {
            get => _addContactInfoPermission;
            protected set => SetProperty<bool>(ref _addContactInfoPermission, value);
        }

        string _avatarURL;
        public string AvatarURL {
            get => _avatarURL;
            private set => SetProperty<string>(ref _avatarURL, value);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _removeContactInfoCancellationTokenSource);

            EditOuterCharacterPopupViewModel?.Dispose();
            AddTeamMemberContactNotePopupViewModel?.Dispose();
        }

        public override Task InitializeAsync(object navigationData) {
            EditOuterCharacterPopupViewModel?.InitializeAsync(navigationData);
            AddTeamMemberContactNotePopupViewModel?.InitializeAsync(navigationData);

            return base.InitializeAsync(navigationData);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _removeContactInfoCancellationTokenSource);
        }

        protected override void OnResolveCharacterProfile() {
            RefreshCharacterValues(TargetMember);
            ResolveAddContactInfoPermission(TargetMember);
        }

        protected override void OnResolveTargetTeam() => ResolveEditPermission(TargetTeam);

        private void RefreshCharacterValues(ProfileDTO profile) {
            try {
                AvatarURL = profile.Avatar?.Url;
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                EditPermission = false;
                AddContactInfoPermission = false;
                AvatarURL = null;
            }
        }

        private void ResolveAddContactInfoPermission(ProfileDTO profile) {
            try {
                if (GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Parent &&
                   profile.ParentId.HasValue &&
                   profile.ParentId.Value == GlobalSettings.Instance.UserProfile.Id) {

                    AddContactInfoPermission = true;
                }
                else {
                    AddContactInfoPermission = false;
                }
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                AddContactInfoPermission = false;
            }
        }

        private void ResolveEditPermission(TeamDTO teamDTO) {
            try {
                if ((GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Organization ||
                    GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Coach) &&
                    TargetTeam.Members.Any(member => member.Member.Id == GlobalSettings.Instance.UserProfile.Id) &&
                    TargetMember.Id != GlobalSettings.Instance.UserProfile.Id) {

                    EditPermission = true;
                }
                else {
                    EditPermission = false;
                }
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                EditPermission = false;
            }
        }
    }
}
