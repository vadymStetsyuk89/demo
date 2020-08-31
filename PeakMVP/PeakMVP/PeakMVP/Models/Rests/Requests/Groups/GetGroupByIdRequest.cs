using PeakMVP.Models.Rests.Requests.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Requests.Groups {
    public class GetGroupByIdRequest : BaseRequest, IAuthorization {
        public string AccessToken { get; set; }
    }
}
