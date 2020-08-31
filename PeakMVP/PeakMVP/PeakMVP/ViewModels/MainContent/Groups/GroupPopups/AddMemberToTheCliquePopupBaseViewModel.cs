using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.Base.Popups;
using PeakMVP.ViewModels.MainContent.Groups.Arguments;
using PeakMVP.ViewModels.MainContent.Search;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Groups.GroupPopups {
    public abstract class AddMemberToTheCliquePopupBaseViewModel : PopupBaseViewModel {

        private static readonly string _MEMBER_ALREADY_IN_POSSIBLE_CLIQUE_MEMBERS_LIST = "{0} already in the list";
        protected static readonly string _NO_MEMBERS_THAT_NECESSARY_TO_INVITE_WARNING_MESSAGE = "Find and select members that you want to invite";

        protected ContentPageBaseViewModel _relativeMainContentPageBaseViewModel;

        public AddMemberToTheCliquePopupBaseViewModel() {
            CommonSearchViewModel = ViewModelLocator.Resolve<SearchMembersForGroupViewModel>();

            PossibleGroupMembers = new ObservableCollection<ProfileDTO>();
        }

        public ICommand InviteExternalMemberCommand => new Command(OnInviteExternalMemberCommand);

        public ICommand RemovePossibleGroupMemberCommand => new Command((profileToRemove) => {
            PossibleGroupMembers.Remove((ProfileDTO)profileToRemove);
        });

        public ICommand AttachMemberToTheCliqueCommand => new Command(async () => {
            if (!PossibleGroupMembers.Any()) {
                await DialogService.ToastAsync(string.Format(AddMemberToGroupPopupViewModel._NO_MEMBERS_THAT_NECESSARY_TO_INVITE_WARNING_MESSAGE));
                return;
            }

            OnAddMemberToTheClique();
        });

        private bool _isExternalInviteAvailable;
        public bool IsExternalInviteAvailable {
            get => _isExternalInviteAvailable;
            protected set => SetProperty<bool>(ref _isExternalInviteAvailable, value);
        }

        private bool _isAnyPossibleMembers;
        public bool IsAnyPossibleMembers {
            get => _isAnyPossibleMembers;
            private set => SetProperty<bool>(ref _isAnyPossibleMembers, value);
        }

        private ObservableCollection<ProfileDTO> _possibleGroupMembers;
        public ObservableCollection<ProfileDTO> PossibleGroupMembers {
            get => _possibleGroupMembers;
            set {
                if (_possibleGroupMembers != null) {
                    _possibleGroupMembers.CollectionChanged -= OnPossibleGroupMembersCollectionChanged;
                }

                SetProperty<ObservableCollection<ProfileDTO>>(ref _possibleGroupMembers, value);

                if (_possibleGroupMembers != null) {
                    _possibleGroupMembers.CollectionChanged += OnPossibleGroupMembersCollectionChanged;
                }
            }
        }

        private SearchViewModelBase _commonSearchViewModel;
        public SearchViewModelBase CommonSearchViewModel {
            get => _commonSearchViewModel;
            private set => SetProperty<SearchViewModelBase>(ref _commonSearchViewModel, value);
        }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is MemberSelectedToBeAddedToTheGroupArgs) {
                InitializeMemberSelectedToBeAddedToTheGroupArgs((MemberSelectedToBeAddedToTheGroupArgs)navigationData);
            }
            else {
                if (_relativeMainContentPageBaseViewModel == null) {
                    _relativeMainContentPageBaseViewModel = (ContentPageBaseViewModel)NavigationService.LastPageViewModel;
                }
            }

            IsExternalInviteAvailable = GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Coach || GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Organization;

            return base.InitializeAsync(navigationData);
        }

        public override void Dispose() {
            base.Dispose();

            CommonSearchViewModel?.Dispose();

            if (_possibleGroupMembers != null) {
                _possibleGroupMembers.CollectionChanged -= OnPossibleGroupMembersCollectionChanged;
            }
        }

        protected abstract void OnAddMemberToTheClique();

        protected virtual void OnInviteExternalMemberCommand() { }

        protected override void OnIsPopupVisible() {
            base.OnIsPopupVisible();

            if (!IsPopupVisible) {
                ResetInputForm();
                Dispose();
            }
        }

        protected virtual void ResetInputForm() {
            CommonSearchViewModel.ResetSearchInputOutputValues();
            PossibleGroupMembers = new ObservableCollection<ProfileDTO>();
        }

        private async void InitializeMemberSelectedToBeAddedToTheGroupArgs(MemberSelectedToBeAddedToTheGroupArgs memberSelected) {
            if (!(PossibleGroupMembers.Any((pDTO) => pDTO.Id == memberSelected.SelectedMember.UserProfile.Id))) {
                PossibleGroupMembers.Add(memberSelected.SelectedMember.UserProfile);
            }
            else {
                await DialogService.ToastAsync(string.Format(AddMemberToTheCliquePopupBaseViewModel._MEMBER_ALREADY_IN_POSSIBLE_CLIQUE_MEMBERS_LIST, memberSelected.SelectedMember.UserProfile.DisplayName));
            }
        }

        private void OnPossibleGroupMembersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            IsAnyPossibleMembers = PossibleGroupMembers.Any();
        } 
    }
}
