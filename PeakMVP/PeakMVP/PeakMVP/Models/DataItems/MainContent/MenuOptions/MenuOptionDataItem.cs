using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Services.DataItems.MainContent;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.DataItems.MainContent {
    public class MenuOptionDataItem {

        public string Title { get; set; }

        public bool IsSelected { get; set; }

        public Type TargetViewModelType { get; set; }
    }
}
