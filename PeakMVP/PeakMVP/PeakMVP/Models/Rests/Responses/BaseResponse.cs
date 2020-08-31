using Newtonsoft.Json;
using PeakMVP.Models.Rests.Responses.Contracts;
using System.Runtime.Serialization;

namespace PeakMVP.Models.Rests.Responses {
    [DataContract]
    public abstract class BaseResponse<TBody> : IResponse {
       
        [DataMember(Name = "Body")]
        public abstract TBody Body { get; set; }

        [JsonProperty("")]
        public string[] Errors { get; set; }
    }
}
