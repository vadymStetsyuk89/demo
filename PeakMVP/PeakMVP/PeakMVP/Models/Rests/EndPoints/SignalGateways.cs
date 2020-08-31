namespace PeakMVP.Models.Rests.EndPoints {
    public class SignalGateways {

        private const string _STATE_GATEWAY = "ws/servicehub?access_token={0}";
        private const string _MESSAGES_GATEWAY = "/ws/imhub?access_token={0}";
        private const string _GROUP_MESSAGING_GATEWAY = "/ws/groupimhub?access_token={0}";

        public SignalGateways(string host) {
            UpdateEndpoint(host);
        }

        public string StateSocketGateway { get; set; }

        public string MessagesSocketGateway { get; set; }

        public string GroupMessagesSocketGateway { get; set; }

        private void UpdateEndpoint(string host) {
            StateSocketGateway = string.Format("{0}{1}", host, _STATE_GATEWAY);
            MessagesSocketGateway = string.Format("{0}{1}", host, _MESSAGES_GATEWAY);
            GroupMessagesSocketGateway = string.Format("{0}{1}", host, _GROUP_MESSAGING_GATEWAY);
        }
    }
}
