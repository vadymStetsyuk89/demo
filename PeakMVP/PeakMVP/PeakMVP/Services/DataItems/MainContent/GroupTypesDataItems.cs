using PeakMVP.Models.DataItems.MainContent.Groups;
using PeakMVP.Models.Identities.Groups;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Services.DataItems.MainContent {
    public class GroupTypesDataItems : IGroupTypesDataItems {

        public static readonly string _TEAM_GROUP_TYPE_DESCRIPTION = "Team";
        public static readonly string _REGULAR_GROUP_TYPE_DESCRIPTION = "Regular";

        public IEnumerable<GroupTypeDataItem> BuildDataItems() {
            return new GroupTypeDataItem[] {
                new GroupTypeDataItem() {
                    GroupType = GroupType.Team,
                    TypeDescription = GroupTypesDataItems._TEAM_GROUP_TYPE_DESCRIPTION
                },
                new GroupTypeDataItem() {
                    GroupType = GroupType.Regular,
                    TypeDescription = GroupTypesDataItems._REGULAR_GROUP_TYPE_DESCRIPTION
                }
            };
        }
    }
}
