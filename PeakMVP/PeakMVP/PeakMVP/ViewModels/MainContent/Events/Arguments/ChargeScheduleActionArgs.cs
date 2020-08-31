using PeakMVP.Models.Identities.Scheduling;
using System;

namespace PeakMVP.ViewModels.MainContent.Events.Arguments {
    public class ChargeScheduleActionArgs {

        public DateTime StartDate { get; set; }

        public ScheduleAction ScheduleAction { get; set; }


        //public TeamMemberDTO RelativeTeamMember { get; set; }

        //public TeamActionBaseDTO ScheduleAction { get; set; }
    }
}
