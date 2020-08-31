using PeakMVP.Models.Rests.DTOs;

namespace PeakMVP.Models.Arguments.InitializeArguments.Profile {
    public class EditOuterProfileArgs {

        public ProfileDTO TargetProfile { get; set; }

        public long RelatedTeamId { get; set; }
    }
}
