using PeakMVP.Models.DataItems.MainContent;
using PeakMVP.Models.DataItems.MainContent.Search;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.Search;
using PeakMVP.Models.Rests.Responses.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Factories.MainContent {
    public interface IFoundUserGroupDataItemFactory {

        List<FoundGroupDataItem> BuildFoundGroupDataItems(SearchResponse searchResponse);
    }
}
