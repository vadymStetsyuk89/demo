using PeakMVP.Models.Arguments.InitializeArguments.Search;
using PeakMVP.Models.DataItems.MainContent.Search;
using PeakMVP.Services.Search;
using PeakMVP.ViewModels.MainContent.Teams;
using System.Linq;

namespace PeakMVP.ViewModels.MainContent.Search {
    public class CommonActionBarSearchViewModel : SearchViewModelBase {

        private static readonly uint _SEARCH_WORD_MIN_LENGTH_FOR_COMMON_SEARCH = 3;

        public CommonActionBarSearchViewModel(
            ISearchService searchService)
            : base(searchService) {
            SearchWordMinLength = CommonActionBarSearchViewModel._SEARCH_WORD_MIN_LENGTH_FOR_COMMON_SEARCH;
        }

        protected async override void OnSelectedSingleUser() {
            base.OnSelectedSingleUser();

            if (SelectedSingleUser is FoundSingleUserDataItem selectedUser) {
                if (selectedUser.UserProfile.Id == GlobalSettings.Instance.UserProfile.Id) {
                    await NavigationService.NavigateToAsync<MainAppViewModel>(new ViewSelfInfoArgs());
                }
                else {
                    if (NavigationService.CurrentViewModelsNavigationStack.LastOrDefault() is ProfileInfoViewModel) {
                        await NavigationService.CurrentViewModelsNavigationStack.LastOrDefault().InitializeAsync(SelectedSingleUser);
                    }
                    else {
                        await NavigationService.NavigateToAsync<ProfileInfoViewModel>(SelectedSingleUser);
                    }
                }
            }
            else if (SelectedSingleUser is FoundSingleTeamDataItem) {
                if (NavigationService.CurrentViewModelsNavigationStack.LastOrDefault() is TeamsInfoViewModel) {
                    await NavigationService.CurrentViewModelsNavigationStack.LastOrDefault().InitializeAsync(((FoundSingleTeamDataItem)SelectedSingleUser).Team);
                }
                else {
                    await NavigationService.NavigateToAsync<TeamsInfoViewModel>(((FoundSingleTeamDataItem)SelectedSingleUser).Team);
                }
            }

            SelectedSingleUser = null;
        }
    }
}
