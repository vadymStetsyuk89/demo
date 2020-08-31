using PeakMVP.Models.Identities.Feed;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Internals;

namespace PeakMVP.Factories.MainContent {
    public class PostPublicityScopeFactory : IPostPublicityScopeFactory {

        private static readonly string _NULL_TEAM_NAME_STUMB = "null_team_name";
        public static readonly string PUBLIC_SCOPE_TITLE = "Public";
        public static readonly string FAMILY_SCOPE_TITLE = "Family";

        public List<PostPublicityScope> BuildCompletedPublicityScopeList(IEnumerable<PostPublicityScope> publicityItems) {
            PostPublicityScope[] onlyPublic = new PostPublicityScope[] { new PostPublicityScope() { Title = PUBLIC_SCOPE_TITLE, Id = 0, PolicyType = PostPolicyType.Public } };

            return (publicityItems == null)
                ? onlyPublic.ToList()
                : onlyPublic.Concat(publicityItems).ToList();
        }

        public List<PostPublicityScope> BuildCompletedPublicityScopeList(IEnumerable<PostPublicityScope> publicityItems, PostPublicityScope familyscopeItem) {
            List<PostPublicityScope> publicityScope = new List<PostPublicityScope>() {
                new PostPublicityScope() { Title = PUBLIC_SCOPE_TITLE, Id = 0 }
            };

            if (familyscopeItem != null) {
                publicityScope.Add(familyscopeItem);
            }

            return (publicityItems == null)
                ? publicityScope
                : publicityScope.Concat(publicityItems).ToList();
        }

        public List<PostPublicityScope> BuildRawPublicityScope(IEnumerable<GroupDTO> groupsData) {
            List<PostPublicityScope> rawScopes = new List<PostPublicityScope>();

            if (groupsData != null) {
                groupsData.ForEach<GroupDTO>(gDTO => {
                    rawScopes.Add(new PostPublicityScope() {
                        Title = gDTO.Name,
                        Id = gDTO.Id,
                        PolicyType = PostPolicyType.Group
                    });
                });
            }

            return rawScopes;
        }

        public List<PostPublicityScope> BuildRawPublicityScope(IEnumerable<TeamMember> teamMemberData) {
            List<PostPublicityScope> rawScopes = new List<PostPublicityScope>();

            if (teamMemberData != null) {
                teamMemberData.ForEach<TeamMember>(tmDTO => {
                    rawScopes.Add(new PostPublicityScope() {
                        Title = (tmDTO.Team != null) ? tmDTO.Team.Name : _NULL_TEAM_NAME_STUMB,
                        Id = (tmDTO.Team != null) ? tmDTO.Team.Id : 0,
                        PolicyType = PostPolicyType.Team
                    });
                });
            }

            return rawScopes;
        }

        public PostPublicityScope BuildFamilyPublicityScope(FamilyDTO familyDTO) {
            PostPublicityScope postPublicityScope = null;

            if (familyDTO != null) {
                postPublicityScope = new PostPublicityScope() {
                    Id = familyDTO.Id,
                    Title = FAMILY_SCOPE_TITLE,
                    PolicyType = PostPolicyType.Family
                };
            }

            return postPublicityScope;
        }
    }
}
