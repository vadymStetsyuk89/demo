using PeakMVP.Models.Identities;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.Responses.IdentityResponses;
using PeakMVP.Services.Profile;
using PeakMVP.ViewModels.MainContent.ProfileContent;
using System;
using System.Collections.Generic;

namespace PeakMVP.Factories.MainContent {
    public class ChildrenFactory : IChildrenFactory {

        private static readonly string IMPERSONATE_IMAGE_PATH = "PeakMVP.Images.ic_impersonate.png";

        private readonly IProfileService _profileService;
        private readonly IProfileFactory _profileFactory;

        public ChildrenFactory(
            IProfileService profileService,
            IProfileFactory profileFactory) {
            _profileService = profileService;
            _profileFactory = profileFactory;
        }

        public ChildItemViewModel MakeChild(RegistrationResponse registrationResponse) {
            ChildItemViewModel childItemViewModel = new ChildItemViewModel(_profileService) {
                Id = registrationResponse.Id,
                Name = registrationResponse.FirstName,
                Age = (DateTime.Now.Year - registrationResponse.DateOfBirth.Year).ToString(),
                //Impersonate = (DateTime.Now.Year - registrationResponse.DateOfBirth.Year) < AGE_RESTRICTION ? IMPERSONATE_IMAGE_PATH : string.Empty,
                IsImpersonateLoginEnabled = (DateTime.Now.Year - registrationResponse.DateOfBirth.Year) < UserProfile.YOUNG_PLAYERS_AGE_LIMIT,
                Profile = _profileFactory.BuildProfileDTO(registrationResponse)
            };

            childItemViewModel.IsAddEmailCommandEnabled = (!childItemViewModel.IsImpersonateLoginEnabled && !registrationResponse.IsEmailConfirmed);

            return childItemViewModel;
        }

        public List<ChildItemViewModel> MakeChildrenItems(IEnumerable<ProfileDTO> profiles) {
            List<ChildItemViewModel> childrenItems = new List<ChildItemViewModel>();

            foreach (var item in profiles) {
                ChildItemViewModel childItemViewModel = new ChildItemViewModel(_profileService) {
                    Id = item.Id,
                    Name = item.FirstName,
                    Age = (DateTime.Now.Year - item.DateOfBirth.Year).ToString(),
                    //Impersonate = (DateTime.Now.Year - item.DateOfBirth.Year) < AGE_RESTRICTION ? IMPERSONATE_IMAGE_PATH : string.Empty
                    IsImpersonateLoginEnabled = (DateTime.Now.Year - item.DateOfBirth.Year) < UserProfile.YOUNG_PLAYERS_AGE_LIMIT,
                    Profile = item
                };

                childItemViewModel.IsAddEmailCommandEnabled = (!childItemViewModel.IsImpersonateLoginEnabled && !item.IsEmailConfirmed);

                childrenItems.Add(childItemViewModel);
            }

            return childrenItems;
        }
    }
}
