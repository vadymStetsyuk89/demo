using PeakMVP.Models.Identities.Messenger;
using PeakMVP.Models.Rests.DTOs.Base;
using PeakMVP.ViewModels.MainContent.Invites;

namespace PeakMVP.Models.Rests.DTOs {
    public class ProfileDTO : ProfileBase, IPossibleMessaging, IInviteTo { }
}



//using Newtonsoft.Json;
//using PeakMVP.Models.Identities.Messenger;
//using PeakMVP.ViewModels.MainContent.Invites;
//using System;

//namespace PeakMVP.Models.Rests.DTOs {
//    public class ProfileDTO : IPossibleMessaging, IInviteTo {
//        [JsonProperty("newMessageIds")]
//        public string[] NewMessageIds { get; set; }

//        [JsonProperty("about")]
//        public string About { get; set; }

//        [JsonProperty("mySports")]
//        public string MySports { get; set; }

//        [JsonProperty("childrens")]
//        public ProfileDTO[] Children { get; set; }

//        [JsonProperty("type")]
//        public string Type { get; set; }

//        [JsonProperty("firstName")]
//        public string FirstName { get; set; }

//        [JsonProperty("lastName")]
//        public string LastName { get; set; }

//        [JsonProperty("displayName")]
//        public string DisplayName { get; set; }

//        [JsonProperty("shortId")]
//        public string ShortId { get; set; }

//        [JsonProperty("isEmailConfirmed")]
//        public bool IsEmailConfirmed { get; set; }

//        [JsonProperty("dateOfBirth")]
//        public DateTime DateOfBirth { get; set; }

//        [JsonProperty("avatars")]
//        public ProfileMediaDTO[] Avatars { get; set; }

//        [JsonProperty("availability")]
//        public string Availability { get; set; }

//        [JsonProperty("lastSeen")]
//        public DateTime? LastSeen { get; set; }

//        [JsonProperty("parentId")]
//        public long? ParentId { get; set; }

//        [JsonProperty("childUserName")]
//        public string ChildUserName { get; set; }

//        [JsonProperty("childPassword")]
//        public string ChildPassword { get; set; }

//        [JsonProperty("contact")]
//        public ContactDTO Contact { get; set; }

//        [JsonProperty("address")]
//        public AddressDTO Address { get; set; }

//        [JsonProperty("backgroundImage")]
//        public ProfileMediaDTO BackgroundImage { get; set; }

//        [JsonProperty("id")]
//        public long Id { get; set; }
//    }
//}
