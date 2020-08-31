using PeakMVP.Models.DataItems.MainContent.Groups;
using PeakMVP.Services.DataItems.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Services.DataItems.MainContent {
    public interface IGroupTypesDataItems {
        IEnumerable<GroupTypeDataItem> BuildDataItems();
    }
}
