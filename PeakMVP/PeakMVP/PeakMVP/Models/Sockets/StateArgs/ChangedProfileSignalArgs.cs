using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;

namespace PeakMVP.Models.Sockets.StateArgs {
    public class ChangedProfileSignalArgs {

        public ProfileDTO Profile { get; set; }
    }
}
