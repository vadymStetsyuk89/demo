using System;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.Base;

namespace PeakMVP.Models.Identities {
    public sealed class UserProfile : ProfileBase {

        public static readonly int YOUNG_PLAYERS_AGE_LIMIT = 13;

        public readonly string EXPIRED_TEST_ACCESS_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIzIiwianRpIjoiZmE5MDFkY2MtZWE5Yi00MzUyLWE5ZjYtMDQ0MzE2Y2Q3ZmYwIiwicHJvZmlsZS1pZCI6WyIzIiwiMyJdLCJwcm9maWxlLXR5cGUiOlsiQ29hY2giLCJDb2FjaCJdLCJleHAiOjE1MzUwMjc1NTMsImlzcyI6IlBlYWtBcGkiLCJhdWQiOiJQZWFrQXBpIn0.-UgD0YzxKmjH2kWDWXaHYs8r92PF-3ueCpsD-jjXPhI";
        public readonly string DEFAULT_AVATAR = "PeakMVP.Images.ic_profile-avatar_white.png";

        public string AccesToken { get; set; }

        public ProfileType ProfileType { get; set; }

        public MediaDTO AppBackgroundImage { get; set; }

        public UserProfile ImpersonateProfile { get; set; }

        public void ClearProfile() {
            About = null;
            MySports = null;
            Children = null;
            Id = 0;
            Type = null;
            FirstName = null;
            LastName = null;
            DisplayName = null;
            ShortId = null;
            IsEmailConfirmed = false;
            DateOfBirth = default(DateTime);
            Avatar = null;
            Availability = null;
            LastSeen = null;
            ParentId = null;
            ChildUserName = null;
            ChildPassword = null;
            Contact = null;
            Address = null;
            BackgroundImage = null;
            BrandImage = null;
            Media = null;
            AccesToken = null;
            ProfileType = default(ProfileType);
            AppBackgroundImage = null;
            ImpersonateProfile = null;
        }
    }
}
