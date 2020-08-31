using PeakMVP.Models.DataItems.MainContent;
using PeakMVP.Models.DataItems.MainContent.Search;
using PeakMVP.Models.Rests.Responses.Search;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeakMVP.Services.Search {
    public interface ISearchService {
        Task<IEnumerable<FoundGroupDataItem>> SearchAsync(string value = "", string type = "");

        Task<SearchResponse> SearchFriendsAsync(string value = "", string type = "", string profileId = "", string profileType = "");
    }
}
