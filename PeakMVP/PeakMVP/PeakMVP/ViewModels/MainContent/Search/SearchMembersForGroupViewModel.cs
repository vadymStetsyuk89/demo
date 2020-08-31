using System.Collections.Generic;
using System.Linq;
using PeakMVP.Factories.MainContent;
using PeakMVP.Models.DataItems.MainContent.Search;
using PeakMVP.Services.Search;
using PeakMVP.ViewModels.MainContent.Groups.Arguments;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.Search {
    public class SearchMembersForGroupViewModel : SearchViewModelBase {

        private static readonly string _ONLY_USER_CAN_BE_ADDED_WARNING = "Team can't be invited, only users";

        public SearchMembersForGroupViewModel(
            ISearchService searchService)
            : base(searchService) { }

        private bool _isMembersSearchPopoverVisible;
        public bool IsMembersSearchPopoverVisible {
            get => _isMembersSearchPopoverVisible;
            set => SetProperty<bool>(ref _isMembersSearchPopoverVisible, value);
        }

        protected async override void OnSelectedSingleUser() {
            base.OnSelectedSingleUser();

            if (SelectedSingleUser is FoundSingleUserDataItem) {
                await NavigationService.LastPageViewModel.InitializeAsync(new MemberSelectedToBeAddedToTheGroupArgs() { SelectedMember = (FoundSingleUserDataItem)SelectedSingleUser });

                IsMembersSearchPopoverVisible = false;
            }
            else {
                await DialogService.ToastAsync(SearchMembersForGroupViewModel._ONLY_USER_CAN_BE_ADDED_WARNING);
            }

            SelectedSingleUser = null;
        }

        protected override void ApplySearchResults(IEnumerable<FoundGroupDataItem> foundGroups) {
            List<FoundGroupDataItem> filteredGroups = null;

            ///
            /// Filterhing teams and self instances
            /// 
            if (foundGroups != null) {
                foundGroups.ForEach(group => {
                    if (group.FoundUsers != null) {
                        group.FoundUsers = group.FoundUsers.OfType<FoundSingleUserDataItem>().Where<FoundSingleUserDataItem>(foundUser => foundUser.UserProfile?.Id != GlobalSettings.Instance.UserProfile.Id).ToList<FoundSingleDataItemBase>();
                    }
                });
                filteredGroups = foundGroups.Where<FoundGroupDataItem>(group => (group.GroupType != FoundUserGroupDataItemFactory.TEAM_TYPE_CONSTANT_VALUE && group.FoundUsers.Any())).ToList();
            }

            base.ApplySearchResults(filteredGroups);
        }
    }
}
