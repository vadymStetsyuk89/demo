using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.Models.Rests.EndPoints {
    public class FamilyEndpoints {

        private static readonly string _GET_FAMILY_API_KEY = "api/family";

        public FamilyEndpoints(string host) {
            UpdateEndpoints(host);
        }

        public string GetFamilyEndpoint { get; private set; }

        private void UpdateEndpoints(string host) {
            GetFamilyEndpoint = string.Format("{0}{1}", host, _GET_FAMILY_API_KEY);
        }
    }
}
