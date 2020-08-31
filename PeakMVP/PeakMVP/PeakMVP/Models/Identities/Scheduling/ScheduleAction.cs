using PeakMVP.Models.DataItems.MainContent.EventsGamesSchedule;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using System;
using System.Linq;
using Xamarin.Forms;

namespace PeakMVP.Models.Identities.Scheduling {
    public class ScheduleAction {

        private static readonly string _UNSUPORTED_SCHEDULED_EVENT_STUB = "Unsuported scheduled event type";
        private static readonly string _GAME_ACTION_TYPE = "game";
        private static readonly string _EVENT_ACTION_TYPE = "event";

        public ScheduleAction(TeamActionBaseDTO scheduledAction, TeamMember relativeTeam) {
            try {
                RelativeTeamMember = relativeTeam;
                ScheduledAction = scheduledAction;

                if (scheduledAction is GameDTO gameAction) {
                    ActionType = _GAME_ACTION_TYPE;
                    Header = string.Format("{0} vs {1}", relativeTeam.Team.Name, gameAction.Opponent.Name);
                    IsRepeatable = false;

                    if (gameAction.IsTimeTbd) {
                        Time.Spans.Add(new Span() { Text = "TBD" });
                    }
                    else {
                        Time.Spans.Add(new Span() { Text = string.Format("{0:hh:mm}", gameAction.StartDate).ToUpper() });
                        Time.Spans.Last().Text = Time.Spans.Last().Text.StartsWith("0") ? Time.Spans.Last().Text.Substring(1) : Time.Spans.Last().Text;
                        Time.Spans.Add(new Span() { Text = " - " });
                        Time.Spans.Add(new Span() { Text = string.Format("{0:hh:mm tt}", gameAction.StartDate + TimeSpan.FromMinutes(gameAction.DurationInMinutes)).ToUpper() });
                        Time.Spans.Last().Text = Time.Spans.Last().Text.StartsWith("0") ? Time.Spans.Last().Text.Substring(1) : Time.Spans.Last().Text;
                    }
                }
                else if (scheduledAction is EventDTO eventAction) {
                    ActionType = _EVENT_ACTION_TYPE;
                    Header = eventAction.Name;
                    IsRepeatable = (eventAction.RepeatingType == ActionRepeatsDataItem.NOT_REPEAT_VALUE) ? false : true;

                    if (eventAction.IsTimeTbd) {
                        Time.Spans.Add(new Span() { Text = "TBD" });
                    }
                    else {
                        Time.Spans.Add(new Span() { Text = string.Format("{0:hh:mm}", eventAction.StartDate).ToUpper() });
                        Time.Spans.Last().Text = Time.Spans.Last().Text.StartsWith("0") ? Time.Spans.Last().Text.Substring(1) : Time.Spans.Last().Text;
                        Time.Spans.Add(new Span() { Text = " - " });
                        Time.Spans.Add(new Span() { Text = string.Format("{0:hh:mm tt}", eventAction.StartDate + TimeSpan.FromMinutes(eventAction.DurationInMinutes)).ToUpper() });
                        Time.Spans.Last().Text = Time.Spans.Last().Text.StartsWith("0") ? Time.Spans.Last().Text.Substring(1) : Time.Spans.Last().Text;
                    }
                }
                else {
                    ActionType = _UNSUPORTED_SCHEDULED_EVENT_STUB;
                    Header = _UNSUPORTED_SCHEDULED_EVENT_STUB;
                }
            }
            catch (Exception exc) {
                throw new InvalidOperationException("ScheduleAction.Ctor()", exc);
            }
        }

        public TeamMember RelativeTeamMember { get; private set; }

        public TeamActionBaseDTO ScheduledAction { get; private set; }

        public string ActionType { get; private set; }

        public string Header { get; private set; }

        public bool IsRepeatable { get; private set; }

        public FormattedString Time { get; private set; } = new FormattedString();
    }
}
