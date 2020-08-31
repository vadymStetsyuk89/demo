using PeakMVP.Models.DataItems.Autorization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PeakMVP.Services.DataItems.Autorization {
    public class FooterDataItems : IFooterDataItems<FooterDataItem> {

        private static readonly string _HOME_ITEM_TITLE = "Home";
        private static readonly string _ABOUT_US_ITEM_TITLE = "About Us";
        private static readonly string _PLAYERS_ITEM_TITLE = "Players";
        private static readonly string _PARENTS_ITEM_TITLE = "Parents";
        private static readonly string _COACHES_ITEM_TITLE = "Coaches";
        private static readonly string _CONTACT_US_ITEM_TITLE = "Contact Us";

        /// <summary>
        /// TODO: for each data item, set appropriate view models type (for navigation)
        /// </summary>
        public ObservableCollection<FooterDataItem> BuildDataItems() =>
            new ObservableCollection<FooterDataItem>() {
                new FooterDataItem() {
                    Title = FooterDataItems._HOME_ITEM_TITLE
                },
                new FooterDataItem() {
                    Title = FooterDataItems._ABOUT_US_ITEM_TITLE
                },
                new FooterDataItem() {
                    Title = FooterDataItems._PLAYERS_ITEM_TITLE
                },
                new FooterDataItem() {
                    Title = FooterDataItems._PARENTS_ITEM_TITLE
                },
                new FooterDataItem() {
                    Title = FooterDataItems._COACHES_ITEM_TITLE
                },
                new FooterDataItem() {
                    Title = FooterDataItems._CONTACT_US_ITEM_TITLE
                }
            };
    }
}
