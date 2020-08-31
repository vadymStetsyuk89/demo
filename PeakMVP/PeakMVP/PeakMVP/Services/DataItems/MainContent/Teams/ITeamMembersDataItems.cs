using PeakMVP.Models.DataItems.MainContent.Teams;
using System.Collections.Generic;

namespace PeakMVP.Services.DataItems.MainContent.Teams {
    public interface ITeamMembersDataItems {

        List<TeamMemberFilterDataItem> BuildTeamMemberFilterItems();
    }
}
