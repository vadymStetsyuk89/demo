using PeakMVP.Models.DataItems.MainContent.EventsGamesSchedule;
using System.Collections.Generic;
using System.Windows.Input;

namespace PeakMVP.Services.DataItems.MainContent.EventsGamesSchedule {
    public interface ITeamActionsManagmentDataItems {

        List<TeamActionManagmentDataItem> BuildActionCreatingDataItems(ICommand selectCommand);

        List<ActionVenueDataItem> BuildActionVenueDataItems();

        List<ActionRepeatsDataItem> BuildRepeatingsDataItems();

        List<AssignmentStatusDataItem> BuildAssignmentStatusDataItems();
    }
}
