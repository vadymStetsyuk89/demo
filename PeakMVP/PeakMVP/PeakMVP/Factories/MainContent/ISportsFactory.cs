using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Rests.DTOs;
using System.Collections.Generic;

namespace PeakMVP.Factories.MainContent {
    public interface ISportsFactory {
        List<SportsDataItem> CreateDataItems(IEnumerable<SportDTO> sportDTOs);
    }
}
