using Autofac;
using PeakMVP.Factories.MainContent;
using PeakMVP.Factories.MainContent.Invitations;
using PeakMVP.Factories.MainContent.Messaging;
using PeakMVP.Factories.Validation;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Services.Cancellation;
using PeakMVP.Services.Chat;
using PeakMVP.Services.Connection;
using PeakMVP.Services.DataItems.Autorization;
using PeakMVP.Services.DataItems.MainContent;
using PeakMVP.Services.DataItems.MainContent.EventsGamesSchedule;
using PeakMVP.Services.DataItems.MainContent.Teams;
using PeakMVP.Services.Dialog;
using PeakMVP.Services.Family;
using PeakMVP.Services.Friends;
using PeakMVP.Services.Groups;
using PeakMVP.Services.Identity;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Services.Invites;
using PeakMVP.Services.MediaPicker;
using PeakMVP.Services.Navigation;
using PeakMVP.Services.OpenUrl;
using PeakMVP.Services.Posts;
using PeakMVP.Services.Profile;
using PeakMVP.Services.ProfileMedia;
using PeakMVP.Services.RequestProvider;
using PeakMVP.Services.Scheduling;
using PeakMVP.Services.Search;
using PeakMVP.Services.SignalR.GroupMessaging;
using PeakMVP.Services.SignalR.Messages;
using PeakMVP.Services.SignalR.StateNotify;
using PeakMVP.Services.Sports;
using PeakMVP.Services.TeamMembers;
using PeakMVP.Services.Teams;
using PeakMVP.ViewModels.Authorization;
using PeakMVP.ViewModels.Authorization.Popups;
using PeakMVP.ViewModels.Authorization.Registration;
using PeakMVP.ViewModels.MainContent;
using PeakMVP.ViewModels.MainContent.ActionBars.Top;
using PeakMVP.ViewModels.MainContent.ActionBars.Top.Popups;
using PeakMVP.ViewModels.MainContent.Albums;
using PeakMVP.ViewModels.MainContent.Character;
using PeakMVP.ViewModels.MainContent.Character.Popups;
using PeakMVP.ViewModels.MainContent.Events;
using PeakMVP.ViewModels.MainContent.Events.EventsPopups;
using PeakMVP.ViewModels.MainContent.Friends;
using PeakMVP.ViewModels.MainContent.Groups;
using PeakMVP.ViewModels.MainContent.Groups.GroupPopups;
using PeakMVP.ViewModels.MainContent.Invites.ChildrenInvites;
using PeakMVP.ViewModels.MainContent.Live;
using PeakMVP.ViewModels.MainContent.Live.ScheduledEventInfo;
using PeakMVP.ViewModels.MainContent.Live.ScheduledEventInfo.Popups;
using PeakMVP.ViewModels.MainContent.MediaViewers;
using PeakMVP.ViewModels.MainContent.Members;
using PeakMVP.ViewModels.MainContent.Messenger;
using PeakMVP.ViewModels.MainContent.Messenger.MessengerTabs;
using PeakMVP.ViewModels.MainContent.ProfileContent;
using PeakMVP.ViewModels.MainContent.ProfileContent.Popups;
using PeakMVP.ViewModels.MainContent.ProfileSettings;
using PeakMVP.ViewModels.MainContent.ProfileSettings.ProfileSettingsPopups;
using PeakMVP.ViewModels.MainContent.ProfileSettings.SelfInformations;
using PeakMVP.ViewModels.MainContent.Search;
using PeakMVP.ViewModels.MainContent.Teams;
using System;
using System.Globalization;
using System.Reflection;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.Base {
    public static class ViewModelLocator {
        private static IContainer _container;

        public static readonly BindableProperty AutoWireViewModelProperty =
            BindableProperty.CreateAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), default(bool), propertyChanged: OnAutoWireViewModelChanged);

        public static bool GetAutoWireViewModel(BindableObject bindable) {
            return (bool)bindable.GetValue(AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(BindableObject bindable, bool value) {
            bindable.SetValue(AutoWireViewModelProperty, value);
        }

        public static void RegisterDependencies() {
            ContainerBuilder builder = new ContainerBuilder();

            // View models.
            builder.RegisterType<HomeViewModel>();
            builder.RegisterType<LoginViewModel>();
            builder.RegisterType<GroupsViewModel>();
            builder.RegisterType<FriendsViewModel>();
            builder.RegisterType<SettingsViewModel>();
            builder.RegisterType<GroupInfoViewModel>();
            builder.RegisterType<DashboardViewModel>();
            builder.RegisterType<CreateFeedViewModel>();
            builder.RegisterType<GroupsPopupViewModel>();
            builder.RegisterType<VideoViewerViewModel>();
            builder.RegisterType<PostContentViewModel>();
            builder.RegisterType<ProfileInfoViewModel>();
            builder.RegisterType<RegistrationViewModel>();
            builder.RegisterType<ManageVideosViewModel>();
            builder.RegisterType<CreateProfileViewModel>();
            builder.RegisterType<EditPostPopupViewModel>();
            builder.RegisterType<ManagePicturesViewModel>();
            builder.RegisterType<ProfileContentViewModel>();
            builder.RegisterType<PicturesViewerViewModel>();
            builder.RegisterType<CommonActionBarViewModel>();
            builder.RegisterType<PickChildAvatarPopupViewModel>();
            builder.RegisterType<PickProfileAvatarPopupViewModel>();
            builder.RegisterType<FanProfileContentViewModel>();
            builder.RegisterType<FanSelfInformationViewModel>();
            builder.RegisterType<CoachProfileContentViewModel>();
            builder.RegisterType<ParentProfileContentViewModel>();
            builder.RegisterType<PlayerProfileContentViewModel>();
            builder.RegisterType<PlayerProfileContentViewModel>();
            builder.RegisterType<CoachSelfInformationViewModel>();
            builder.RegisterType<ParentSelfInformationViewModel>();
            builder.RegisterType<AddMemberToGroupPopupViewModel>();
            builder.RegisterType<CommonActionBarSearchViewModel>();
            builder.RegisterType<SearchMembersForGroupViewModel>();
            builder.RegisterType<PlayerSelfInformationViewModel>();
            builder.RegisterType<FanRegistrationInputFormViewModel>();
            builder.RegisterType<CoachRegistrationInputFormViewModel>();
            builder.RegisterType<OrganizationProfileContentViewModel>();
            builder.RegisterType<OrganizationSelfInformationViewModel>();
            builder.RegisterType<PlayerRegistrationInputFormViewModel>();
            builder.RegisterType<ParentRegistrationInputFormViewModel>();
            builder.RegisterType<OrganizationRegistrationInputFormViewModel>();
            builder.RegisterType<TeamsInfoViewModel>();
            builder.RegisterType<AddMemberToTeamPopupViewModel>();
            builder.RegisterType<AddTeamPopupViewModel>();
            builder.RegisterType<AddChildrenPopupViewModel>();
            builder.RegisterType<MessengerViewModel>();
            builder.RegisterType<FriendsMessengerTabViewModel>();
            builder.RegisterType<GroupsMessengerTabViewModel>();
            builder.RegisterType<TeamsMessengerTabViewModel>();
            builder.RegisterType<FamilyMessengerTabViewModel>();
            builder.RegisterType<ConversationTabViewModel>();
            builder.RegisterType<ModeActionBarViewModel>();
            builder.RegisterType<MainAppViewModel>();
            builder.RegisterType<CharacterViewModel>();
            builder.RegisterType<AlbumsViewModel>();
            builder.RegisterType<HamburgerMenuPopupViewModel>();
            builder.RegisterType<TeamsViewModel>();
            builder.RegisterType<MembersViewModel>();
            builder.RegisterType<EventsViewModel>();
            builder.RegisterType<LiveViewModel>();
            builder.RegisterType<InvitesContentViewModel>();
            builder.RegisterType<CalendarViewingOfEventsAndGamesContentViewModel>();
            builder.RegisterType<ListViewingOfEventsAndGamesContentViewModel>();
            builder.RegisterType<CreateNewGameViewModel>();
            builder.RegisterType<AssignmentViewModel>();
            builder.RegisterType<AddOpponentPopupViewModel>();
            builder.RegisterType<AddLocationPopupViewModel>();
            builder.RegisterType<EditGameViewModel>();
            builder.RegisterType<CreateNewEventViewModel>();
            builder.RegisterType<EditEventViewModel>();
            builder.RegisterType<CalendarScheduleEventViewModel>();
            builder.RegisterType<ViewDayAppointmentsPopupViewModel>();
            builder.RegisterType<TeamMemberProviderViewModel>();
            builder.RegisterType<MediaActionPopupViewModel>();
            builder.RegisterType<ForgotPasswordViewModel>();
            builder.RegisterType<ChildInviteToGroupItemViewModel>();
            builder.RegisterType<ChildFriendshipInviteItemViewModel>();
            builder.RegisterType<ChildInviteToTeamItemViewModel>();
            builder.RegisterType<CharacterDetailInfoViewModel>();
            builder.RegisterType<InviteExternalMembersViewModel>();
            builder.RegisterType<ExternalInviteActionBarViewModel>();
            builder.RegisterType<EditOuterCharacterPopupViewModel>();
            builder.RegisterType<ChildSettingsUpdateViewModel>();
            builder.RegisterType<EditCharacterDetailsViewModel>();
            builder.RegisterType<AddTeamMemberContactNotePopupViewModel>();
            builder.RegisterType<ScheduledEventInfoViewModel>();
            builder.RegisterType<ScheduleEventDetailsViewModel>();
            builder.RegisterType<InterestAndAvailabilityViewModel>();
            builder.RegisterType<EventStatisticsViewModel>();
            builder.RegisterType<AddAvailabilityNotePopupViewModel>();

            // Data items.
            builder.RegisterType<GroupTypesDataItems>().As<IGroupTypesDataItems>();
            builder.RegisterType<MenuOptionsDataItems>().As<IMenuOptionsDataItems>();
            builder.RegisterType<FooterDataItems>().As<IFooterDataItems<FooterDataItem>>();
            builder.RegisterType<CreateProfileDataItems>().As<ICreateProfileDataItems<ProfileTypeItem>>();
            builder.RegisterType<MessengerDataItems>().As<IMessengerDataItems>();
            builder.RegisterType<TeamActionsManagmentDataItems>().As<ITeamActionsManagmentDataItems>();
            builder.RegisterType<TeamMembersDataItems>().As<ITeamMembersDataItems>();

            // Services.
            builder.RegisterType<TeamService>().As<ITeamService>();
            builder.RegisterType<PostService>().As<IPostService>();
            builder.RegisterType<ChatService>().As<IChatService>();
            builder.RegisterType<SportService>().As<ISportService>().SingleInstance();
            builder.RegisterType<FamilyService>().As<IFamilyService>();
            builder.RegisterType<SearchService>().As<ISearchService>();
            builder.RegisterType<FriendService>().As<IFriendService>();
            builder.RegisterType<GroupsService>().As<IGroupsService>();
            builder.RegisterType<DialogService>().As<IDialogService>();
            builder.RegisterType<InviteService>().As<IInviteService>();
            builder.RegisterType<OpenUrlService>().As<IOpenUrlService>();
            builder.RegisterType<ProfileService>().As<IProfileService>();
            builder.RegisterType<RequestProvider>().As<IRequestProvider>().SingleInstance();
            builder.RegisterType<IdentityService>().As<IIdentityService>();
            builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();
            builder.RegisterType<NavigationContext>().As<INavigationContext>().SingleInstance();
            builder.RegisterType<ConnectionService>().As<IConnectionService>();
            builder.RegisterType<TeamMemberService>().As<ITeamMemberService>();
            builder.RegisterType<MediaPickerService>().As<IMediaPickerService>().SingleInstance();
            builder.RegisterType<CancellationService>().As<ICancellationService>();
            builder.RegisterType<ProfileMediaService>().As<IProfileMediaService>();
            builder.RegisterType<IdentityUtilService>().As<IIdentityUtilService>().SingleInstance();
            builder.RegisterType<StateService>().As<IStateService>().SingleInstance();
            builder.RegisterType<MessagesService>().As<IMessagesService>().SingleInstance();
            builder.RegisterType<SchedulingService>().As<ISchedulingService>();
            builder.RegisterType<GroupMessagingService>().As<IGroupMessagingService>().SingleInstance();

            // Factories.
            builder.RegisterType<TeamFactory>().As<ITeamFactory>();
            builder.RegisterType<InvitationsFactory>().As<IInvitationsFactory>();
            builder.RegisterType<MediaFactory>().As<IMediaFactory>();
            builder.RegisterType<SportsFactory>().As<ISportsFactory>();
            builder.RegisterType<FriendFactory>().As<IFriendFactory>();
            builder.RegisterType<FileDTOBuilder>().As<IFileDTOBuilder>();
            builder.RegisterType<ChildrenFactory>().As<IChildrenFactory>();
            builder.RegisterType<TeamMemberFactory>().As<ITeamMemberFactory>();
            builder.RegisterType<MessageItemFactory>().As<IMessageItemFactory>();
            builder.RegisterType<GroupingFactory>().As<IGroupingFactory>();
            builder.RegisterType<ContentViewModelFactory>().As<IContentViewModelFactory>();
            builder.RegisterType<ValidationObjectFactory>().As<IValidationObjectFactory>();
            builder.RegisterType<PostPublicityScopeFactory>().As<IPostPublicityScopeFactory>();
            builder.RegisterType<FoundUserGroupDataItemFactory>().As<IFoundUserGroupDataItemFactory>();
            builder.RegisterType<PossibleConversationItemsFactory>().As<IPossibleConversationItemsFactory>();
            builder.RegisterType<ProfileFactory>().As<IProfileFactory>();

            if (_container != null) {
                _container.Dispose();
            }

            _container = builder.Build();
        }

        public static T Resolve<T>() {
            return _container.Resolve<T>();
        }

        private static void OnAutoWireViewModelChanged(BindableObject bindable, object oldValue, object newValue) {
            var view = bindable as Element;
            if (view == null) {
                return;
            }

            var viewType = view.GetType();
            var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}Model, {1}", viewName, viewAssemblyName);

            var viewModelType = Type.GetType(viewModelName);
            if (viewModelType == null) {
                return;
            }
            var viewModel = _container.Resolve(viewModelType);
            view.BindingContext = viewModel;
        }
    }
}
