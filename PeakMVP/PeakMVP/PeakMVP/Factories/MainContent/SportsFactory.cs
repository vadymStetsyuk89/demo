using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Rests.DTOs;
using System;
using System.Collections.Generic;

namespace PeakMVP.Factories.MainContent {
    public sealed class SportsFactory : ISportsFactory {
        public List<SportsDataItem> CreateDataItems(IEnumerable<SportDTO> source) {
            List<SportsDataItem> sportsDataItems = new List<SportsDataItem>();

            foreach (var item in source) {
                sportsDataItems.Add(new SportsDataItem {
                    Data = item,
                    Name = item.Name,
                    Description = item.Description,
                    SportsType = (SportsType)Enum.Parse(typeof(SportsType), item.Name),
                    Id = item.Id
                });
            }

            return sportsDataItems;
        }
    }
}
