namespace PeakMVP.Models.Rests.EndPoints {
    public class AppEndpoints {

        internal const string HOST = "http://ec2-34-197-79-232.compute-1.amazonaws.com/";

        internal const string HOST_ONLY_WITH_VPN = "http://alpha-stable.competebase.com/";

        public ChatEndPoints ChatEndPoints { get; private set; } = new ChatEndPoints(HOST_ONLY_WITH_VPN);

        public PostEndpoints PostEndpoints { get; private set; } = new PostEndpoints(HOST_ONLY_WITH_VPN);

        public TeamEndPoints TeamEndPoints { get; private set; } = new TeamEndPoints(HOST_ONLY_WITH_VPN);

        public SportEndPoints SportEndPoints { get; private set; } = new SportEndPoints(HOST_ONLY_WITH_VPN);

        public MediaEndPoints MediaEndPoints { get; private set; } = new MediaEndPoints(HOST_ONLY_WITH_VPN);

        public FriendEndPoints FriendEndPoints { get; private set; } = new FriendEndPoints(HOST_ONLY_WITH_VPN);

        public GroupsEndpoints GroupsEndpoints { get; private set; } = new GroupsEndpoints(HOST_ONLY_WITH_VPN);

        public SearchEndPoints SearchEndPoints { get; private set; } = new SearchEndPoints(HOST_ONLY_WITH_VPN);

        public InviteEndPoints InviteEndPoints { get; private set; } = new InviteEndPoints(HOST_ONLY_WITH_VPN);

        public ProfileEndpoints ProfileEndpoints { get; private set; } = new ProfileEndpoints(HOST_ONLY_WITH_VPN);

        public TeamMemberEndPoints TeamMemberEndPoints { get; private set; } = new TeamMemberEndPoints(HOST_ONLY_WITH_VPN);

        public ProfileMediaEndPoints ProfileMediaEndPoints { get; private set; } = new ProfileMediaEndPoints(HOST_ONLY_WITH_VPN);

        public AuthenticationEndpoints AuthenticationEndpoints { get; private set; } = new AuthenticationEndpoints(HOST_ONLY_WITH_VPN);

        public FamilyEndpoints FamilyEndpoints { get; private set; } = new FamilyEndpoints(HOST_ONLY_WITH_VPN);

        public SignalGateways SignalGateways { get; private set; } = new SignalGateways(HOST_ONLY_WITH_VPN);

        public ScheduleEndpoints ScheduleEndpoints { get; private set; } = new ScheduleEndpoints(HOST_ONLY_WITH_VPN);
    }
}
