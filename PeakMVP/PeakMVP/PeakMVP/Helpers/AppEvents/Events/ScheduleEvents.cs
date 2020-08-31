using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.ViewModels.MainContent.Events;
using PeakMVP.ViewModels.MainContent.Events.Arguments;
using System;

namespace PeakMVP.Helpers.AppEvents.Events {
    public class ScheduleEvents {

        public event EventHandler<AssignmentViewModel> ActionAssignmentRemoved = delegate { };
        public event EventHandler<OpponentDTO> NewOpponentCreated = delegate { };
        public event EventHandler<LocationDTO> NewLocationCreated = delegate { };

        public event EventHandler<GameManagmentArgs> NewGameCreated = delegate { };
        public event EventHandler<GameManagmentArgs> GameDeleted = delegate { };
        public event EventHandler<GameManagmentArgs> GameUpdated = delegate { };

        public event EventHandler<EventManagmentArgs> EventDeleted = delegate { };
        public event EventHandler<EventManagmentArgs> EventCreated = delegate { };
        public event EventHandler<EventManagmentArgs> EventUpdated = delegate { };

        public event EventHandler<ScheduledCalendarDateSelectedArgs> CalendarScheduleDateSelected = delegate { };

        public void InvokeActionAssignmentRemoved(object sender, AssignmentViewModel removedAssignment) => ActionAssignmentRemoved(sender, removedAssignment);
        public void InvokeNewOpponentCreated(object sender, OpponentDTO createdOpponent) => NewOpponentCreated(sender, createdOpponent);
        public void InvokeNewLocationCreated(object sender, LocationDTO createdLocation) => NewLocationCreated(sender, createdLocation);

        public void InvokeNewGameCreated(object sender, GameManagmentArgs args) => NewGameCreated(sender, args);
        public void InvokeGameDeleted(object sender, GameManagmentArgs args) => GameDeleted(sender, args);
        public void InvokeGameUpdated(object sender, GameManagmentArgs args) => GameUpdated(sender, args);

        public void InvokeEventDeleted(object sender, EventManagmentArgs args) => EventDeleted(sender, args);
        public void InvokeEventCreated(object sender, EventManagmentArgs args) => EventCreated(sender, args);
        public void InvokeEventUpdated(object sender, EventManagmentArgs args) => EventUpdated(sender, args);

        public void InvokeCalendarScheduleDateSelected(object sender, ScheduledCalendarDateSelectedArgs args) => CalendarScheduleDateSelected(sender, args);
    }

    public class GameManagmentArgs : EventArgs {

        public GameDTO Game { get; set; }

        public TeamMember TeamMember { get; set; }
    }

    public class EventManagmentArgs : EventArgs {

        public EventDTO Event { get; set; }

        public TeamMember TeamMember { get; set; }
    }
}
