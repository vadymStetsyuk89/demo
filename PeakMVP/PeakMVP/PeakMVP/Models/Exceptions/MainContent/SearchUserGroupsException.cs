using System;

namespace PeakMVP.Models.Exceptions.MainContent {
    public class SearchUserGroupsException : Exception {

        public string Error { get; set; }

        public string ErrorDescription { get; set; }
    }
}
