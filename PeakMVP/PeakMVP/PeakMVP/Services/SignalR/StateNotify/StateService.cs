using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Sockets.StateArgs;
using System;
using System.Diagnostics;

namespace PeakMVP.Services.SignalR.StateNotify {
    public class StateService : SignalServiceBase, IStateService {

        public static readonly string CHANGED_INVITES_HANDLING_ERROR = "Can't handle changed invite";
        public static readonly string CHANGED_GROUPS_HANDLING_ERROR = "Can't handle changed groups";
        public static readonly string CHANGED_TEAMS_HANDLING_ERROR = "Can't handle changed teams";
        public static readonly string CHANGED_FRIENDSHIP_HANDLING_ERROR = "Can't handle changed friendship";
        public static readonly string CHANGED_PROFILE_HANDLING_ERROR = "Can't handle changed profile";

        private static readonly string _CHANGED_PROFILE_ACTION_KEY = "ChangedProfile";
        private static readonly string _CHANGED_FRIENDSHIP_ACTION_KEY = "ChangedFriendship";
        private static readonly string _CHANGED_TEAMS_ACTION_KEY = "ChangedTeams";
        private static readonly string _CHANGED_GROUPS_ACTION_KEY = "ChangedGroups";
        private static readonly string _CHANGED_INVITES_ACTION_KEY = "ChangedInvites";

        public event EventHandler<ChangedInvitesSignalArgs> InvitesChanged = delegate { };
        public event EventHandler<ChangedFriendshipSignalArgs> ChangedFriendship = delegate { };
        public event EventHandler<ChangedGroupsSignalArgs> ChangedGroups = delegate { };
        public event EventHandler<ChangedTeamsSignalArgs> ChangedTeams = delegate { };
        public event EventHandler<ChangedProfileSignalArgs> ChangedProfile = delegate { };

        public override string SocketHub { get; protected set; } = GlobalSettings.Instance.Endpoints.SignalGateways.StateSocketGateway;

        protected override void OnStartListeningToHub() {
            _hubConnection.On<object>(_CHANGED_PROFILE_ACTION_KEY, (args) => {
                Console.WriteLine("===> {0}", _CHANGED_PROFILE_ACTION_KEY);

                try {
                    ChangedProfileSignalArgs changedProfileSignalArgs = new ChangedProfileSignalArgs() { Profile = JsonConvert.DeserializeObject<ProfileDTO>(args.ToString()) };
                    ChangedProfile.Invoke(this, changedProfileSignalArgs);
                }
                catch (Exception exc) {
                    Debugger.Break();

                    throw new InvalidOperationException(CHANGED_PROFILE_HANDLING_ERROR, exc);
                }
            });

            _hubConnection.On<object>(_CHANGED_FRIENDSHIP_ACTION_KEY, (args) => {
                Console.WriteLine("===> {0}", _CHANGED_FRIENDSHIP_ACTION_KEY);

                try {
                    ChangedFriendshipSignalArgs changedFriendshipSignalArgs = JsonConvert.DeserializeObject<ChangedFriendshipSignalArgs>(args.ToString());
                    ChangedFriendship.Invoke(this, changedFriendshipSignalArgs);
                }
                catch (Exception exc) {
                    Debugger.Break();

                    throw new InvalidOperationException(CHANGED_FRIENDSHIP_HANDLING_ERROR, exc);
                }
            });

            _hubConnection.On<object>(_CHANGED_TEAMS_ACTION_KEY, (args) => {
                Console.WriteLine("===> {0}", _CHANGED_TEAMS_ACTION_KEY);

                try {
                    ChangedTeamsSignalArgs changedTeamsSignalArgs = JsonConvert.DeserializeObject<ChangedTeamsSignalArgs>(args.ToString());
                    ChangedTeams.Invoke(this, changedTeamsSignalArgs);
                }
                catch (Exception exc) {
                    Debugger.Break();

                    throw new InvalidOperationException(CHANGED_TEAMS_HANDLING_ERROR, exc);
                }
            });

            _hubConnection.On<object>(_CHANGED_GROUPS_ACTION_KEY, (args) => {
                Console.WriteLine("===> {0}", _CHANGED_GROUPS_ACTION_KEY);

                try {
                    ChangedGroupsSignalArgs changedGroupsSignalArgs = JsonConvert.DeserializeObject<ChangedGroupsSignalArgs>(args.ToString());
                    ChangedGroups.Invoke(this, changedGroupsSignalArgs);
                }
                catch (Exception exc) {
                    Debugger.Break();

                    throw new InvalidOperationException(CHANGED_GROUPS_HANDLING_ERROR, exc);
                }
            });

            _hubConnection.On<object>(_CHANGED_INVITES_ACTION_KEY, (args) => {
                Console.WriteLine("===> {0}", _CHANGED_INVITES_ACTION_KEY);

                try {
                    ChangedInvitesSignalArgs changedInvitesSignalArgs = JsonConvert.DeserializeObject<ChangedInvitesSignalArgs>(args.ToString());
                    InvitesChanged.Invoke(this, changedInvitesSignalArgs);
                }
                catch (Exception exc) {
                    Debugger.Break();

                    throw new InvalidOperationException(CHANGED_INVITES_HANDLING_ERROR, exc);
                }
            });
        }
    }
}
