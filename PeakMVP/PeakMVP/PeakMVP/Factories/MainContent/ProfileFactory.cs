using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.IdentityResponses;
using PeakMVP.Models.Rests.Responses.ProfileResponses;
using PeakMVP.Models.Rests.Responses.Teams;

namespace PeakMVP.Factories.MainContent {
    public class ProfileFactory : IProfileFactory {

        public static readonly string PHONE_NOT_SPECIFIED_STUB = "Phone not specified";
        public static readonly string EMAIL_NOT_SPECIFIED_STUB = "Email not specified";
        public static readonly string ADDRESS_NOT_SPECIFIED_STUB = "Address not specified";

        public ProfileDTO BuildProfileDTO(NixTeamAppointmentResponse data) {
            ProfileDTO result = new ProfileDTO() {
                About = data.About,
                MySports = data.MySports,
                Children = data.Children,
                Id = data.Id,
                Type = data.Type,
                FirstName = data.FirstName,
                LastName = data.LastName,
                DisplayName = data.DisplayName,
                ShortId = data.ShortId,
                DateOfBirth = data.DateOfBirth,
                Availability = data.Availability,
                LastSeen = data.LastSeen,
                ParentId = data.ParentId,
                ChildUserName = data.ChildUserName,
                ChildPassword = data.ChildPassword,
                Contact = data.Contact,
                Address = data.Address,
                BackgroundImage = data.BackgroundImage,
                Avatar = data.Avatar,
                BrandImage = data.BrandImage,
                IsEmailConfirmed = data.IsEmailConfirmed,
                Media = data.Media
            };

            return result;
        }

        public ProfileDTO BuildProfileDTO(RegistrationResponse data) {
            ProfileDTO result = new ProfileDTO() {
                About = data.About,
                MySports = data.MySports,
                Children = data.Children,
                Id = data.Id,
                Type = data.Type,
                FirstName = data.FirstName,
                LastName = data.LastName,
                DisplayName = data.DisplayName,
                ShortId = data.ShortId,
                DateOfBirth = data.DateOfBirth,
                Availability = data.Availability,
                LastSeen = data.LastSeen,
                ParentId = data.ParentId,
                ChildUserName = data.ChildUserName,
                ChildPassword = data.ChildPassword,
                Contact = data.Contact,
                Address = data.Address,
                BackgroundImage = data.BackgroundImage,
                Avatar = data.Avatar,
                BrandImage = data.BrandImage,
                Media = data.Media,
                IsEmailConfirmed = data.IsEmailConfirmed
            };

            return result;
        }

        public ProfileDTO BuildProfileDTO(GetProfileResponse data) {
            ProfileDTO result = new ProfileDTO() {
                About = data.About,
                MySports = data.MySports,
                Children = data.Children,
                Id = data.Id,
                Type = data.Type,
                FirstName = data.FirstName,
                LastName = data.LastName,
                DisplayName = data.DisplayName,
                ShortId = data.ShortId,
                DateOfBirth = data.DateOfBirth,
                Availability = data.Availability,
                LastSeen = data.LastSeen,
                ParentId = data.ParentId,
                ChildUserName = data.ChildUserName,
                ChildPassword = data.ChildPassword,
                Contact = data.Contact,
                Address = data.Address,
                BackgroundImage = data.BackgroundImage,
                Avatar = data.Avatar,
                BrandImage = data.BrandImage,
                Media = data.Media,
                IsEmailConfirmed = data.IsEmailConfirmed
            };

            return result;
        }
    }
}
