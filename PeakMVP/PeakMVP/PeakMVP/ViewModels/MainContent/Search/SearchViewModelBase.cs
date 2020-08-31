using Microsoft.AppCenter.Crashes;
using PeakMVP.Models.DataItems.MainContent.Search;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Exceptions.MainContent;
using PeakMVP.Services.Search;
using PeakMVP.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.MainContent.Search {
    public abstract class SearchViewModelBase : ViewModelBase {

        private static readonly string _TYPE_SOMETHING_HINT_TEXT = "Type at least 3 symbols to search...";
        private static readonly string _READY_TO_SEARCH_HINT_TEXT = "Tap button to search";
        private static readonly string _NO_MATCHING_RESULTS_HINT_TEXT = "No Matching Results";

        private readonly ISearchService _searchService;

        private CancellationTokenSource _searchCancellationTokenSource = new CancellationTokenSource();

        public SearchViewModelBase(
            ISearchService searchService) {

            _searchService = searchService;

            SearchValue = "";

            try {
                Observable.FromEventPattern<PropertyChangedEventArgs>(this, nameof(PropertyChanged))
                    .Where(x => x.EventArgs.PropertyName == nameof(SearchValue))
                    .Throttle(TimeSpan.FromMilliseconds(700))
                    .Select(handler => Observable.FromAsync(async cancellationToken => {
                        var result = await SearchUsersAsync(SearchValue).ConfigureAwait(false);

                        if (cancellationToken.IsCancellationRequested) {
                            return new FoundGroupDataItem[] { };
                        }

                        return result;
                    }))
                    .Switch()
                    .Subscribe(foundResult => {
                        ApplySearchResults(foundResult);
                    });
            }
            catch (Exception ex) {
                Debug.WriteLine($"---ERROR: {ex.Message}");
                IsSearchBusy = false;
            }
        }

        public ICommand StartSuggestionCommand => new Command(async () => {
            FoundUsersGroups = null;
            SelectedUsersGroup = null;

            if (StartSuggestionCommand.CanExecute(null)) {
                ApplySearchResults(await SearchUsersAsync(SearchValue));
            }
        }, IsReadyToSearch);

        public ICommand BackToFoundUserGroupsCommand => new Command(() => SelectedUsersGroup = null);

        public uint SearchWordMinLength { get; protected set; }

        private bool _isSearchBusy = false;
        public bool IsSearchBusy {
            get => _isSearchBusy;
            private set => SetProperty<bool>(ref _isSearchBusy, value);
        }

        private bool _isStepBackVisible;
        public bool IsStepBackVisible {
            get => _isStepBackVisible;
            private set => SetProperty<bool>(ref _isStepBackVisible, value);
        }

        private string _searchValue;
        public string SearchValue {
            get => _searchValue;
            set {
                SetProperty<string>(ref _searchValue, value);

                SuggestInputHintText = "";
                ((Command)StartSuggestionCommand).ChangeCanExecute();
            }
        }

        private string _suggestInputHintText;
        public string SuggestInputHintText {
            get => _suggestInputHintText;
            set => SetProperty<string>(ref _suggestInputHintText, value);
        }

        private IEnumerable<FoundGroupDataItem> _foundUsersGroups;
        public IEnumerable<FoundGroupDataItem> FoundUsersGroups {
            get => _foundUsersGroups;
            private set {
                SetProperty<IEnumerable<FoundGroupDataItem>>(ref _foundUsersGroups, value);

                SelectedSingleUser = null;
                SelectedUsersGroup = null;
            }
        }

        private FoundGroupDataItem _selectedUsersGroup;
        public FoundGroupDataItem SelectedUsersGroup {
            get => _selectedUsersGroup;
            set {
                SetProperty<FoundGroupDataItem>(ref _selectedUsersGroup, value);

                SelectedSingleUser = null;

                IsFoundGroupSelected = (value != null);
                IsStepBackVisible = (FoundUsersGroups != null && FoundUsersGroups.Count() > 1);
            }
        }

        private FoundSingleDataItemBase _selectedSingleUser;
        public FoundSingleDataItemBase SelectedSingleUser {
            get => _selectedSingleUser;
            set {
                SetProperty<FoundSingleDataItemBase>(ref _selectedSingleUser, value);

                if (value != null) {
                    OnSelectedSingleUser();
                }
            }
        }

        private bool _isFoundGroupSelected;
        public bool IsFoundGroupSelected {
            get => _isFoundGroupSelected;
            set => SetProperty<bool>(ref _isFoundGroupSelected, value);
        }

        public void ResetSearchInputOutputValues() {
            SearchValue = "";
            FoundUsersGroups = new FoundGroupDataItem[] { };
            SelectedUsersGroup = null;
            SelectedSingleUser = null;
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _searchCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _searchCancellationTokenSource);
        }

        protected virtual void OnSelectedSingleUser() { }

        protected virtual void ApplySearchResults(IEnumerable<FoundGroupDataItem> foundGroups) {
            if (foundGroups != null) {
                FoundUsersGroups = foundGroups;

                if (FoundUsersGroups.Count() == 1) {
                    SelectedUsersGroup = FoundUsersGroups.First();
                }
            }
            else {
                FoundUsersGroups = new FoundGroupDataItem[] { };
            }

            SuggestInputHintText = ((string.IsNullOrEmpty(SearchValue)) || (FoundUsersGroups != null && FoundUsersGroups.Any<FoundGroupDataItem>()))
                ? "" : SearchViewModelBase._NO_MATCHING_RESULTS_HINT_TEXT;
        }

        private bool IsReadyToSearch() {
            return (SearchValue.Length >= SearchWordMinLength);
        }

        private async Task<IEnumerable<FoundGroupDataItem>> SearchUsersAsync(string searchString) {
            IsSearchBusy = true;

            ResetCancellationTokenSource(ref _searchCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _searchCancellationTokenSource;

            IEnumerable<FoundGroupDataItem> foundResult = null;

            if (!string.IsNullOrEmpty(searchString)) {
                try {
                    foundResult = await _searchService.SearchAsync(searchString);

                    if (foundResult.Count() > 1) {
                        foundResult.Skip(1).ForEach(fUG => fUG.IsHaveSeparator = true);
                        foundResult.ForEach(fUG => fUG.FoundUsers.ForEach(fSU => fSU.IsHaveSeparator = true));
                    }

                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
                catch (SearchUserGroupsException searchExc) {
                    Crashes.TrackError(searchExc);

                    await DialogService.ToastAsync(string.Format("{0}.{1}", searchExc.Error, searchExc.ErrorDescription));
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    Debugger.Break();
                    return null;
                }
            }
            else {
                foundResult = new FoundGroupDataItem[] { };
            }

            IsSearchBusy = false;

            return foundResult;
        }
    }
}