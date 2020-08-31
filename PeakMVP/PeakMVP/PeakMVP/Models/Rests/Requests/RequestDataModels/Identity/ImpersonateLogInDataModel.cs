using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Requests.RequestDataModels.Identity {
    public class ImpersonateLogInDataModel {

        [JsonProperty("childProfileId")]
        public long ChildProfileId { get; set; }
    }
}
