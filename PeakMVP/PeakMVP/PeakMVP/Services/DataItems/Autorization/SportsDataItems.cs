using PeakMVP.Models.DataItems.Autorization;
using System.Collections.ObjectModel;

namespace PeakMVP.Services.DataItems.Autorization {
    public class SportsDataItems : ISportsDataItems<SportsDataItem> {

        private static readonly string HOCKEY_CONSTANT_VALUE = "Hockey";
        private static readonly string BASEBALL_CONSTANT_VALUE = "Baseball";
        private static readonly string BASKETBALL_CONSTANT_VALUE = "Basketball";
        private static readonly string VOLLEYBALL_CONSTANT_VALUE = "Volleyball";
        private static readonly string TENNIS_CONSTANT_VALUE = "Tennis";
        private static readonly string LACROSSE_CONSTANT_VALUE = "Lacrosse";
        private static readonly string SOCCER_CONSTANT_VALUE = "Soccer";

        public ObservableCollection<SportsDataItem> BuildDataItems() {
            return new ObservableCollection<SportsDataItem>() {
                new SportsDataItem() {
                    Description = "Hockey",
                    SportsType = SportsType.Hockey,
                    Name=SportsDataItems.HOCKEY_CONSTANT_VALUE
                },
                new SportsDataItem() {
                    Description = "Baseball",
                    SportsType = SportsType.Baseball,
                    Name=SportsDataItems.BASEBALL_CONSTANT_VALUE
                },
                new SportsDataItem() {
                    Description = "Basketball",
                    SportsType = SportsType.Basketball,
                    Name=SportsDataItems.BASKETBALL_CONSTANT_VALUE
                },
                new SportsDataItem() {
                    Description = "Volleyball",
                    SportsType = SportsType.Volleyball,
                    Name=SportsDataItems.VOLLEYBALL_CONSTANT_VALUE
                },
                new SportsDataItem() {
                    Description = "Tennis",
                    SportsType = SportsType.Tennis,
                    Name=SportsDataItems.TENNIS_CONSTANT_VALUE
                },
                new SportsDataItem() {
                    Description = "Lacrosse",
                    SportsType = SportsType.Lacrosse,
                    Name=SportsDataItems.LACROSSE_CONSTANT_VALUE
                },
                new SportsDataItem() {
                    Description = "Soccer",
                    SportsType = SportsType.Soccer,
                    Name=SportsDataItems.SOCCER_CONSTANT_VALUE
                }
            };
        }
    }
}
