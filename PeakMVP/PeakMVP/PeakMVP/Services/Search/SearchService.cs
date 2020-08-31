using PeakMVP.Factories.MainContent;
using PeakMVP.Models.DataItems.MainContent;
using PeakMVP.Models.Exceptions.MainContent;
using PeakMVP.Models.Rests.Requests.Search;
using PeakMVP.Models.Rests.Responses.Search;
using PeakMVP.Services.RequestProvider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using PeakMVP.Models.Rests.DTOs;
using System.Diagnostics;
using Xamarin.Forms.Internals;
using Microsoft.AppCenter.Crashes;
using PeakMVP.Services.IdentityUtil;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.DataItems.MainContent.Search;
using Plugin.Connectivity;
using PeakMVP.Helpers;

namespace PeakMVP.Services.Search {
    public class SearchService : ISearchService {

        private readonly IRequestProvider _requestProvider;
        private readonly IFoundUserGroupDataItemFactory _foundUserGroupDataItemFactory;
        private readonly IIdentityUtilService _identityUtilService;

        /// <summary>
        ///     ctor().
        /// </summary>
        public SearchService(
            IRequestProvider requestProvider,
            IFoundUserGroupDataItemFactory foundUserGroupDataItemFactory,
            IIdentityUtilService identityUtilService) {

            _requestProvider = requestProvider;
            _foundUserGroupDataItemFactory = foundUserGroupDataItemFactory;
            _identityUtilService = identityUtilService;
        }

        public async Task<IEnumerable<FoundGroupDataItem>> SearchAsync(string value = "", string type = "") =>
            await Task<IEnumerable<FoundGroupDataItem>>.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                SearchRequest searchRequest = new SearchRequest {
                    Url = string.Format(GlobalSettings.Instance.Endpoints.SearchEndPoints.SimpleSearchEndPoints, value, type),
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken
                };

                SearchResponse searchResponse = null;
                IEnumerable<FoundGroupDataItem> foundUserGroups = null;

                try {
                    searchResponse = await _requestProvider.GetAsync<SearchRequest, SearchResponse>(searchRequest);

                    if (searchResponse != null) {
                        foundUserGroups = _foundUserGroupDataItemFactory.BuildFoundGroupDataItems(searchResponse);
                    }
                } catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                } catch (Exception ex) {
                    Crashes.TrackError(ex);

                    throw;
                }

                return foundUserGroups;
            });

        public async Task<SearchResponse> SearchFriendsAsync(string value = "", string type = "", string profileId = "", string profileType = "") =>
            await Task.Run(async () => {
                if (!CrossConnectivity.Current.IsConnected) {
                    throw new InvalidOperationException(AppConsts.ERROR_INTERNET_CONNECTION);
                }

                SearchRequest searchRequest = new SearchRequest {
                    Url = string.Format(GlobalSettings.Instance.Endpoints.SearchEndPoints.SearchFriendsEndpoints, value, type, profileId, profileType),
                    AccessToken = GlobalSettings.Instance.UserProfile.AccesToken
                };

                SearchResponse searchResponse = null;

                try {
                    searchResponse = await _requestProvider.GetAsync<SearchRequest, SearchResponse>(searchRequest);
                } catch (ServiceAuthenticationException exc) {
                    _identityUtilService.RefreshToken();

                    throw exc;
                } catch (Exception ex) {
                    Crashes.TrackError(ex);

                    Debug.WriteLine($"ERROR:{ex.Message}");
                    Debugger.Break();
                    throw;
                }
                return searchResponse;
            });
    }
}
