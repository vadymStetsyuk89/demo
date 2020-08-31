using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.DataItems.MainContent;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Services.DataItems.MainContent {
    public interface IMenuOptionsDataItems {
        IEnumerable<MenuOptionDataItem> ResolveMenuOptions(ProfileType profileType);
    }
}
