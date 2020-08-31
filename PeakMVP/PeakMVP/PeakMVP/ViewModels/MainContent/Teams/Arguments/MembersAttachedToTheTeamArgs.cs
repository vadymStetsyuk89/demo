using PeakMVP.Models.Rests.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.ViewModels.MainContent.Teams.Arguments {
    public class MembersAttachedToTheTeamArgs {

        public List<ProfileDTO> AttachedMembers { get; set; }
    }
}
