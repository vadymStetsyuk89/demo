using PeakMVP.Models.DataItems.MainContent.EventsGamesSchedule;
using PeakMVP.Services.Navigation;
using PeakMVP.ViewModels.MainContent.Events;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.Services.DataItems.MainContent.EventsGamesSchedule {
    public class TeamActionsManagmentDataItems : ITeamActionsManagmentDataItems {

        public static readonly string NEW_GAME_ACTION_TITLE = "New Game";
        public static readonly string NEW_EVENT_ACTION_TITLE = "New Event";

        public List<TeamActionManagmentDataItem> BuildActionCreatingDataItems(ICommand selectCommand) =>
            new List<TeamActionManagmentDataItem>() {
                new TeamActionManagmentDataItem() {
                    ActionTitle = NEW_GAME_ACTION_TITLE,
                    SelectCommand = selectCommand
                },
                new TeamActionManagmentDataItem() {
                    ActionTitle = NEW_EVENT_ACTION_TITLE,
                    SelectCommand = selectCommand
                }
            };

        public List<ActionVenueDataItem> BuildActionVenueDataItems() =>
            new List<ActionVenueDataItem>() {
                new ActionVenueDataItem(){
                    ActionVenue = ActionVenueDataItem.AWAY_VENUE_VALUE
                },
                new ActionVenueDataItem(){
                    ActionVenue = ActionVenueDataItem.HOME_VENUE_VALUE
                },
                new ActionVenueDataItem(){
                    ActionVenue = ActionVenueDataItem.UNKNOWN_VENUE_VALUE
                }
            };

        public List<AssignmentStatusDataItem> BuildAssignmentStatusDataItems() =>
            new List<AssignmentStatusDataItem>() {
                new AssignmentStatusDataItem(){
                    Status = AssignmentStatusDataItem.UNASSIGNED_STATUS_VALUE
                },
                new AssignmentStatusDataItem(){
                    Status = AssignmentStatusDataItem.ASSIGNED_STATUS_VALUE
                }
            };

        public List<ActionRepeatsDataItem> BuildRepeatingsDataItems() =>
            new List<ActionRepeatsDataItem>() {
                new ActionRepeatsDataItem(){
                    Header = ActionRepeatsDataItem.NOT_REPEAT_HEADER,
                    Value = ActionRepeatsDataItem.NOT_REPEAT_VALUE
                },
                new ActionRepeatsDataItem(){
                    Header = ActionRepeatsDataItem.DAILY_HEADER,
                    Value = ActionRepeatsDataItem.DAILY_REPEAT_VALUE
                },
                new ActionRepeatsDataItem(){
                    Header = ActionRepeatsDataItem.WEEKLY_HEADER,
                    Value = ActionRepeatsDataItem.WEEKLY_REPEAT_VALUE
                }
            };
    }
}
