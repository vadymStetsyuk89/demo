using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;

namespace PeakMVP.Models.AppNavigation.Character {
    public class ViewTeamMemberCharacterInfoArgs {

        public TeamDTO TargetTeam { get; set; }

        public ProfileDTO TargetMember { get; set; }

        public TeamMember RelativeTeamMember { get; set; }
    }
}
