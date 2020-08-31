using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.MainContent;
using PeakMVP.Models.AppNavigation.Character;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Services.Friends;
using PeakMVP.Services.Profile;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.Services.TeamMembers;
using PeakMVP.Services.Teams;
using PeakMVP.ViewModels.MainContent.Messenger.Arguments;
using Plugin.Messaging;
using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Character {
    public class CharacterDetailInfoViewModel : CharacterInfoViewModelBase {

        public static readonly string CONVERSATION_FOR_FRIENDS_ONLY = "Can't start conversation now. Add this member to the friends first and confirm friendship.";
        private static readonly string _PHONE_NOT_SPECIFIED = "Phone not specified";
        private static readonly string _EMAIL_NOT_SPECIFIED = "Email not specified";
        private static readonly string _CAN_NOT_SENT_SMS_FROM_CURRENT_DEVICE = "Can't sent sms from current device";
        private static readonly string _CAN_NOT_MAKE_A_CALL_FROM_CURRENT_DEVICE = "Can't make a call from current device";
        private static readonly string _CAN_NOT_SEND_EMAIL_FROM_CURRENT_DEVICE = "Can't send email from current device";

        public CharacterDetailInfoViewModel(
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
                  teamMemberService) { }

        public ICommand HeartCommand => new Command(async () => {
            if (TargetMember != null) {
                await NavigationService.NavigateToAsync<ProfileInfoViewModel>(TargetMember);
            }
        });

        public ICommand MessageCommand => new Command(async () => {
            if (TargetMember != null) {
                if (IsFriend) {
                    await NavigationService.GoBackAsync(new StartOuterConversationArgs() { TargetCompanion = TargetMember });
                }
                else {
                    await DialogService.ToastAsync(CONVERSATION_FOR_FRIENDS_ONLY);
                }
            }
        });

        public ICommand EditCharacterCommand => new Command(async () => await NavigationService.NavigateToAsync<EditCharacterDetailsViewModel>(new ViewTeamMemberCharacterInfoArgs() { TargetTeam = TargetTeam, TargetMember = TargetMember, RelativeTeamMember = RelativeTeamMember }));

        public ICommand SMSCommand => new Command(async () => {
            if (!string.IsNullOrEmpty(Phone) && !string.IsNullOrWhiteSpace(Phone)) {
                try {
                    if (CrossMessaging.Current.SmsMessenger.CanSendSms) {
                        CrossMessaging.Current.SmsMessenger.SendSms(Phone, string.Format("Dear {0}", FullName));
                    }
                    else {
                        await DialogService.ToastAsync(_CAN_NOT_SENT_SMS_FROM_CURRENT_DEVICE);
                    }
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(_CAN_NOT_SENT_SMS_FROM_CURRENT_DEVICE);
                }
            }
            else {
                await DialogService.ToastAsync(_PHONE_NOT_SPECIFIED);
            }
        });

        public ICommand PhoneCallCommand => new Command(async () => {
            if (!string.IsNullOrEmpty(Phone) && !string.IsNullOrWhiteSpace(Phone)) {
                try {
                    if (CrossMessaging.Current.PhoneDialer.CanMakePhoneCall) {
                        CrossMessaging.Current.PhoneDialer.MakePhoneCall(Phone);
                    }
                    else {
                        await DialogService.ToastAsync(_CAN_NOT_MAKE_A_CALL_FROM_CURRENT_DEVICE);
                    }
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(_CAN_NOT_MAKE_A_CALL_FROM_CURRENT_DEVICE);
                }
            }
            else {
                await DialogService.ToastAsync(_PHONE_NOT_SPECIFIED);
            }
        });

        public ICommand MailToCommand => new Command(async () => {
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrWhiteSpace(Email)) {
                try {
                    if (CrossMessaging.Current.EmailMessenger.CanSendEmail) {
                        CrossMessaging.Current.EmailMessenger.SendEmail(Email, string.Format("Peack MVP. {0}", TargetTeam.Name), string.Format("Dear {0}", TargetMember.DisplayName));
                    }
                    else {
                        await DialogService.ToastAsync(_CAN_NOT_SEND_EMAIL_FROM_CURRENT_DEVICE);
                    }
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(_CAN_NOT_SEND_EMAIL_FROM_CURRENT_DEVICE);
                }
            }
            else {
                await DialogService.ToastAsync(_EMAIL_NOT_SPECIFIED);
            }
        });

        public ICommand SMSByContactInfoCommand => new Command(async (object param) => {
            if (param is TeamMemberContactInfo memberContactInfo && !string.IsNullOrEmpty(memberContactInfo.FirstPhoneNumber)) {
                try {
                    if (CrossMessaging.Current.SmsMessenger.CanSendSms) {
                        CrossMessaging.Current.SmsMessenger.SendSms(memberContactInfo.FirstPhoneNumber, string.Format("Dear {0}", memberContactInfo.FirstName));
                    }
                    else {
                        await DialogService.ToastAsync(_CAN_NOT_SENT_SMS_FROM_CURRENT_DEVICE);
                    }
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(_CAN_NOT_SENT_SMS_FROM_CURRENT_DEVICE);
                }
            }
            else {
                await DialogService.ToastAsync(_PHONE_NOT_SPECIFIED);
            }
        });

        public ICommand MailToByContactInfoCommand => new Command(async (object param) => {
            if (param is TeamMemberContactInfo memberContactInfo && !string.IsNullOrEmpty(memberContactInfo.Email)) {
                try {
                    if (CrossMessaging.Current.EmailMessenger.CanSendEmail) {
                        CrossMessaging.Current.EmailMessenger.SendEmail(memberContactInfo.Email, string.Format("Peack MVP. {0}", TargetTeam.Name), string.Format("Dear {0}", memberContactInfo.FirstName));
                    }
                    else {
                        await DialogService.ToastAsync(_CAN_NOT_SEND_EMAIL_FROM_CURRENT_DEVICE);
                    }
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(_CAN_NOT_SEND_EMAIL_FROM_CURRENT_DEVICE);
                }
            }
            else {
                await DialogService.ToastAsync(_EMAIL_NOT_SPECIFIED);
            }
        });

        public ICommand PhoneCallByContactInfoCommand => new Command(async (object param) => {
            if (param is TeamMemberContactInfo memberContactInfo && !string.IsNullOrEmpty(memberContactInfo.FirstPhoneNumber)) {
                try {
                    if (CrossMessaging.Current.PhoneDialer.CanMakePhoneCall) {
                        CrossMessaging.Current.PhoneDialer.MakePhoneCall(memberContactInfo.FirstPhoneNumber);
                    }
                    else {
                        await DialogService.ToastAsync(_CAN_NOT_MAKE_A_CALL_FROM_CURRENT_DEVICE);
                    }
                }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(_CAN_NOT_MAKE_A_CALL_FROM_CURRENT_DEVICE);
                }
            }
            else {
                await DialogService.ToastAsync(_PHONE_NOT_SPECIFIED);
            }
        });

        private bool _editPermission;
        public bool EditPermission {
            get => _editPermission;
            protected set => SetProperty<bool>(ref _editPermission, value);
        }

        string _titleInfo;
        public string TitleInfo {
            get => _titleInfo;
            private set => SetProperty<string>(ref _titleInfo, value);
        }

        string _avatarURL;
        public string AvatarURL {
            get => _avatarURL;
            private set => SetProperty<string>(ref _avatarURL, value);
        }

        string _fullName;
        public string FullName {
            get => _fullName;
            private set => SetProperty<string>(ref _fullName, value);
        }

        string _email;
        public string Email {
            get => _email;
            private set => SetProperty<string>(ref _email, value);
        }

        string _phone;
        public string Phone {
            get => _phone;
            private set => SetProperty<string>(ref _phone, value);
        }

        string _address;
        public string Address {
            get => _address;
            private set => SetProperty<string>(ref _address, value);
        }

        protected override void OnResolveCharacterProfile() => RefreshCharacterValues(TargetMember);

        protected override void OnResolveTargetTeam() {
            base.OnResolveTargetTeam();

            ResolveEditPermission(TargetTeam, TargetMember);
        }

        private void ResolveEditPermission(TeamDTO teamDTO, ProfileDTO profile) {
            try {
                if ((GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Organization ||
                    GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Coach) &&
                    TargetTeam.Members.Any(member => member.Member.Id == GlobalSettings.Instance.UserProfile.Id) &&
                    TargetMember.Type == FoundUserGroupDataItemFactory.PLAYER_TYPE_CONSTANT_VALUE &&
                    TargetMember.Id != GlobalSettings.Instance.UserProfile.Id) {

                    EditPermission = true;
                }
                else if (GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Parent &&
                    profile.ParentId.HasValue &&
                    profile.ParentId.Value == GlobalSettings.Instance.UserProfile.Id) {

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

        private void RefreshCharacterValues(ProfileDTO profile) {
            try {
                AvatarURL = profile.Avatar?.Url;
                TitleInfo = string.Format("{0}, {1}", profile.DisplayName, profile.Type);
                FullName = profile.DisplayName;
                Email = profile.Contact?.Email;
                Phone = profile.Contact?.Phone;
                Address = profile.Address != null ? string.Format("{0} {1} {2} {3}", profile.Address.City, profile.Address.State, profile.Address.Zip, profile.Address.Street).Trim() : null;
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                AvatarURL = null;
                TitleInfo = null;
                FullName = null;
                Email = null;
                Phone = null;
                Address = null;

                EditPermission = false;
            }
        }
    }
}




//using Microsoft.AppCenter.Crashes;
//using PeakMVP.Factories.MainContent;
//using PeakMVP.Helpers;
//using PeakMVP.Models.AppNavigation.Character;
//using PeakMVP.Models.DataItems.Autorization;
//using PeakMVP.Models.Exceptions;
//using PeakMVP.Models.Rests.DTOs;
//using PeakMVP.Models.Rests.Responses.Friends;
//using PeakMVP.Models.Sockets.StateArgs;
//using PeakMVP.Services.Friends;
//using PeakMVP.Services.Profile;
//using PeakMVP.Services.SignalR.StateNotify;
//using PeakMVP.Services.Teams;
//using PeakMVP.ViewModels.Base;
//using PeakMVP.ViewModels.MainContent.Character.Popups;
//using PeakMVP.ViewModels.MainContent.Messenger.Arguments;
//using Plugin.Messaging;
//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows.Input;
//using Xamarin.Forms;

//namespace PeakMVP.ViewModels.MainContent.Character {
//    public class CharacterDetailInfoViewModel : LoggedContentPageViewModel {

//        public static readonly string CONVERSATION_FOR_FRIENDS_ONLY = "Can't start conversation now. Add this member to the friends first and confirm friendship.";
//        private static readonly string _PHONE_NOT_SPECIFIED = "Phone not specified";
//        private static readonly string _EMAIL_NOT_SPECIFIED = "Email not specified";
//        private static readonly string _CAN_NOT_SENT_SMS_FROM_CURRENT_DEVICE = "Can't sent sms from current device";
//        private static readonly string _CAN_NOT_MAKE_A_CALL_FROM_CURRENT_DEVICE = "Can't make a call from current device";
//        private static readonly string _CAN_NOT_SEND_EMAIL_FROM_CURRENT_DEVICE = "Can't send email from current device";

//        private readonly IProfileService _profileService;
//        private readonly IProfileFactory _profileFactory;
//        private readonly IStateService _stateService;
//        private readonly ITeamService _teamService;
//        private readonly IFriendService _friendService;

//        private CancellationTokenSource _resolveCharacterProfileCancellationTokenSource = new CancellationTokenSource();
//        private CancellationTokenSource _resolveTargetTeamCancellationTokenSource = new CancellationTokenSource();
//        private CancellationTokenSource _isFriendlyCancellationTokenSource = new CancellationTokenSource();

//        private ProfileDTO _targetMember;
//        private TeamDTO _targetTeam;

//        public CharacterDetailInfoViewModel(
//            IProfileService profileService,
//            IProfileFactory profileFactory,
//            IStateService stateService,
//            ITeamService teamService,
//            IFriendService friendService) {

//            _profileService = profileService;
//            _profileFactory = profileFactory;
//            _stateService = stateService;
//            _teamService = teamService;
//            _friendService = friendService;

//            EditOuterCharacterPopupViewModel = ViewModelLocator.Resolve<EditOuterCharacterPopupViewModel>();
//            EditOuterCharacterPopupViewModel?.InitializeAsync(this);

//            RefreshCommand = new Command(async () => {
//                IsRefreshing = true;

//                await ResolveCharacterProfileAsync();
//                await ResolveTargetTeamAsync();

//                await Task.Delay(AppConsts.DELAY_STUB);
//                IsRefreshing = false;
//            });
//        }

//        public ICommand HeartCommand => new Command(async () => {
//            if (_targetMember != null) {
//                await NavigationService.NavigateToAsync<ProfileInfoViewModel>(_targetMember);
//            }
//        });

//        public ICommand MessageCommand => new Command(async () => {
//            if (_targetMember != null) {
//                if (IsFriend) {
//                    await NavigationService.GoBackAsync(new StartOuterConversationArgs() { TargetCompanion = _targetMember });
//                }
//                else {
//                    await DialogService.ToastAsync(CONVERSATION_FOR_FRIENDS_ONLY);
//                }
//            }
//        });

//        public ICommand EditCharacterCommand => new Command(async () => await NavigationService.NavigateToAsync<EditCharacterDetailsViewModel>());

//        //public ICommand EditCharacterCommand => new Command(() => EditOuterCharacterPopupViewModel?.ShowPopupCommand.Execute(new EditOuterProfileArgs() {
//        //    RelatedTeamId = _targetTeam.Id,
//        //    TargetProfile = _targetMember
//        //}));

//        public ICommand SMSCommand => new Command(async () => {
//            //if (!string.IsNullOrEmpty(Phone) && !string.IsNullOrWhiteSpace(Phone)) {
//            ///
//            /// TODO: only for testing, uncomment upper line
//            /// 
//            if (true) {
//                if (CrossMessaging.Current.SmsMessenger.CanSendSms) {
//                    //CrossMessaging.Current.SmsMessenger.SendSms(Phone, string.Format("Dear {0}", FullName));
//                    ///
//                    /// TODO: only for testing, uncomment upper line
//                    /// 
//                    CrossMessaging.Current.SmsMessenger.SendSms("+1234567890123", string.Format("Dear {0}", FullName));
//                }
//                else {
//                    await DialogService.ToastAsync(_CAN_NOT_SENT_SMS_FROM_CURRENT_DEVICE);
//                }
//            }
//            else {
//                await DialogService.ToastAsync(_PHONE_NOT_SPECIFIED);
//            }
//        });

//        public ICommand PhoneCallCommand => new Command(async () => {
//            //if (!string.IsNullOrEmpty(Phone) && !string.IsNullOrWhiteSpace(Phone)) {
//            ///
//            /// TODO: only for testing, uncomment upper line
//            /// 
//            if (true) {
//                if (CrossMessaging.Current.PhoneDialer.CanMakePhoneCall) {
//                    //CrossMessaging.Current.PhoneDialer.MakePhoneCall(Phone);
//                    ///
//                    /// TODO: only for testing, uncomment upper line
//                    /// 
//                    CrossMessaging.Current.PhoneDialer.MakePhoneCall("+1234567890123");
//                }
//                else {
//                    await DialogService.ToastAsync(_CAN_NOT_MAKE_A_CALL_FROM_CURRENT_DEVICE);
//                }
//            }
//            else {
//                await DialogService.ToastAsync(_PHONE_NOT_SPECIFIED);
//            }
//        });

//        public ICommand MailToCommand => new Command(async () => {
//            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrWhiteSpace(Email)) {
//                if (CrossMessaging.Current.EmailMessenger.CanSendEmail) {
//                    CrossMessaging.Current.EmailMessenger.SendEmail(Email, string.Format("Peack MVP. {0}", _targetTeam.Name), string.Format("Dear {0}", _targetMember.DisplayName));
//                }
//                else {
//                    await DialogService.ToastAsync(_CAN_NOT_SEND_EMAIL_FROM_CURRENT_DEVICE);
//                }
//            }
//            else {
//                await DialogService.ToastAsync(_EMAIL_NOT_SPECIFIED);
//            }
//        });

//        private EditOuterCharacterPopupViewModel _editOuterCharacterPopupViewModel;
//        public EditOuterCharacterPopupViewModel EditOuterCharacterPopupViewModel {
//            get => _editOuterCharacterPopupViewModel;
//            private set => SetProperty<EditOuterCharacterPopupViewModel>(ref _editOuterCharacterPopupViewModel, value);
//        }

//        private bool _isFriend;
//        public bool IsFriend {
//            get => _isFriend;
//            private set => SetProperty<bool>(ref _isFriend, value);
//        }

//        string _titleInfo;
//        public string TitleInfo {
//            get => _titleInfo;
//            private set => SetProperty<string>(ref _titleInfo, value);
//        }

//        string _avatarURL;
//        public string AvatarURL {
//            get => _avatarURL;
//            private set => SetProperty<string>(ref _avatarURL, value);
//        }

//        string _fullName;
//        public string FullName {
//            get => _fullName;
//            private set => SetProperty<string>(ref _fullName, value);
//        }

//        string _email;
//        public string Email {
//            get => _email;
//            private set => SetProperty<string>(ref _email, value);
//        }

//        string _phone;
//        public string Phone {
//            get => _phone;
//            private set => SetProperty<string>(ref _phone, value);
//        }

//        string _address;
//        public string Address {
//            get => _address;
//            private set => SetProperty<string>(ref _address, value);
//        }

//        private bool _editAvailability;
//        public bool EditAvailability {
//            get => _editAvailability;
//            private set => SetProperty<bool>(ref _editAvailability, value);
//        }

//        protected override void LoseIntent() {
//            base.LoseIntent();

//            ResetCancellationTokenSource(ref _resolveCharacterProfileCancellationTokenSource);
//            ResetCancellationTokenSource(ref _resolveTargetTeamCancellationTokenSource);
//            ResetCancellationTokenSource(ref _isFriendlyCancellationTokenSource);
//        }

//        public override void Dispose() {
//            base.Dispose();

//            EditOuterCharacterPopupViewModel?.Dispose();
//            ResetCancellationTokenSource(ref _resolveCharacterProfileCancellationTokenSource);
//            ResetCancellationTokenSource(ref _resolveTargetTeamCancellationTokenSource);
//            ResetCancellationTokenSource(ref _isFriendlyCancellationTokenSource);
//        }

//        public override Task InitializeAsync(object navigationData) {
//            if (navigationData is ViewTeamMemberCharacterInfoArgs characterInfoArgs) {
//                _targetTeam = characterInfoArgs.TargetTeam;
//                _targetMember = characterInfoArgs.TargetTeamMember.Member;

//                ExecuteActionWithBusy(ResolveCharacterProfileAsync);
//                ExecuteActionWithBusy(ResolveTargetTeamAsync);
//                ResolveIsFriendltAsync(_targetMember.Id);
//            }

//            EditOuterCharacterPopupViewModel?.InitializeAsync(navigationData);

//            return base.InitializeAsync(navigationData);
//        }

//        protected override void OnSubscribeOnAppEvents() {
//            base.OnSubscribeOnAppEvents();

//            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.OuterProfileInfoUpdated += OnProfileSettingsEventsOuterProfileInfoUpdated;
//            _stateService.ChangedProfile += OnstateServiceChangedProfile;
//            _stateService.ChangedFriendship += OnStateServiceChangedFriendship;
//            _stateService.InvitesChanged += OnStateServiceInvitesChanged;
//        }

//        protected override void OnUnsubscribeFromAppEvents() {
//            base.OnUnsubscribeFromAppEvents();

//            GlobalSettings.Instance.AppMessagingEvents.ProfileSettingsEvents.OuterProfileInfoUpdated -= OnProfileSettingsEventsOuterProfileInfoUpdated;
//            _stateService.ChangedProfile -= OnstateServiceChangedProfile;
//            _stateService.ChangedFriendship -= OnStateServiceChangedFriendship;
//            _stateService.InvitesChanged -= OnStateServiceInvitesChanged;
//        }

//        private Task ResolveCharacterProfileAsync() =>
//            Task.Run(async () => {
//                if (_targetMember == null) {
//                    return;
//                }

//                ResetCancellationTokenSource(ref _resolveCharacterProfileCancellationTokenSource);
//                CancellationTokenSource cancellationTokenSource = _resolveCharacterProfileCancellationTokenSource;

//                try {
//                    ProfileDTO profile = await _profileService.GetProfileByIdAsync(_targetMember.Id, cancellationTokenSource.Token);
//                    if (profile != null) {
//                        _targetMember = profile;

//                        Device.BeginInvokeOnMainThread(() => {
//                            RefreshCharacterValues(_targetMember);
//                        });
//                    }
//                    else {
//                        throw new InvalidOperationException(ProfileService.CANT_RESOLVE_PROFILE_COMMON_ERROR_MESSAGE);
//                    }
//                }
//                catch (OperationCanceledException) { }
//                catch (ObjectDisposedException) { }
//                catch (ServiceAuthenticationException) { }
//                catch (Exception exc) {
//                    Crashes.TrackError(exc);

//                    await DialogService.ToastAsync(exc.Message);
//                }
//            });


//        private Task ResolveTargetTeamAsync() =>
//            Task.Run(async () => {
//                if (_targetTeam == null) {
//                    return;
//                }

//                ResetCancellationTokenSource(ref _resolveTargetTeamCancellationTokenSource);
//                CancellationTokenSource cancellationTokenSource = _resolveTargetTeamCancellationTokenSource;

//                try {
//                    TeamDTO team = await _teamService.GetTeamByIdAsync(_targetTeam.Id, cancellationTokenSource);

//                    if (team != null) {
//                        _targetTeam = team;

//                        Device.BeginInvokeOnMainThread(() => {
//                            RefreshCharacterAvailability(_targetTeam);
//                        });
//                    }
//                    else {
//                        throw new InvalidOperationException(TeamService.CANT_GET_TEAM_ERROR);
//                    }
//                }
//                catch (OperationCanceledException) { }
//                catch (ObjectDisposedException) { }
//                catch (ServiceAuthenticationException) { }
//                catch (Exception exc) {
//                    Crashes.TrackError(exc);

//                    await DialogService.ToastAsync(exc.Message);
//                }
//            });

//        private Task ResolveIsFriendltAsync(long profileId) =>
//            Task.Run(async () => {

//                ResetCancellationTokenSource(ref _isFriendlyCancellationTokenSource);
//                CancellationTokenSource cancellationTokenSource = _isFriendlyCancellationTokenSource;

//                try {
//                    bool isFriend = false;
//                    GetFriendByIdResponse friend = await _friendService.GetFriendByIdAsync(profileId, cancellationTokenSource.Token);

//                    if (friend != null && friend.Profile != null) {
//                        isFriend = true;
//                    }
//                    else {
//                        isFriend = false;
//                    }

//                    Device.BeginInvokeOnMainThread(() => {
//                        IsFriend = isFriend;
//                    });
//                }
//                catch (OperationCanceledException) { }
//                catch (ObjectDisposedException) { }
//                catch (ServiceAuthenticationException) { }
//                catch (Exception exc) {
//                    Crashes.TrackError(exc);

//                    Device.BeginInvokeOnMainThread(() => {
//                        IsFriend = false;
//                    });
//                }
//            });

//        private void OnstateServiceChangedProfile(object sender, ChangedProfileSignalArgs e) => ResolveCharacterProfileAsync();

//        private void OnStateServiceInvitesChanged(object sender, ChangedInvitesSignalArgs e) {
//            try {
//                ResolveIsFriendltAsync(_targetMember.Id);
//            }
//            catch (Exception exc) {
//                Crashes.TrackError(exc);
//            }
//        }

//        private void OnStateServiceChangedFriendship(object sender, ChangedFriendshipSignalArgs e) {
//            try {
//                ResolveIsFriendltAsync(_targetMember.Id);
//            }
//            catch (Exception exc) {
//                Crashes.TrackError(exc);
//            }
//        }

//        private void RefreshCharacterValues(ProfileDTO profile) {
//            try {
//                AvatarURL = profile.Avatar?.Url;
//                TitleInfo = string.Format("{0}, {1}", profile.DisplayName, profile.Type);
//                FullName = profile.DisplayName;
//                Email = profile.Contact?.Email;
//                Phone = profile.Contact?.Phone;
//                Address = profile.Address != null ? string.Format("{0} {1} {2} {3}", profile.Address.City, profile.Address.State, profile.Address.Zip, profile.Address.Street).Trim() : null;
//            }
//            catch (Exception exc) {
//                Crashes.TrackError(exc);

//                EditAvailability = false;
//                AvatarURL = null;
//                TitleInfo = null;
//                FullName = null;
//                Email = null;
//                Phone = null;
//                Address = null;
//            }
//        }

//        private void RefreshCharacterAvailability(TeamDTO teamDTO) {
//            try {
//                if ((GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Organization ||
//                    GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Coach) &&
//                    _targetTeam.Members.Any(member => member.Member.Id == GlobalSettings.Instance.UserProfile.Id) &&
//                    _targetMember.Id != GlobalSettings.Instance.UserProfile.Id) {

//                    EditAvailability = true;
//                }
//                else {
//                    EditAvailability = false;
//                }
//            }
//            catch (Exception exc) {
//                Crashes.TrackError(exc);

//                EditAvailability = false;
//            }
//        }

//        private async void OnProfileSettingsEventsOuterProfileInfoUpdated(object sender, EventArgs e) {
//            await ResolveCharacterProfileAsync();
//            await ResolveTargetTeamAsync();
//        }
//    }
//}
