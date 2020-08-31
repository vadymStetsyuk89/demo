using PeakMVP.Models.Rests.Requests.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.Requests.ProfileMedia {
    public class AddProfileMediaRequest : BaseRequest, IAuthorization {

        public string AccessToken { get; set; }
    }
}
