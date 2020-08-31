using PeakMVP.Models.Sockets.StateArgs;
using System;

namespace PeakMVP.Services.SignalR.StateNotify {
    public interface IStateService : ISignalService {

        event EventHandler<ChangedInvitesSignalArgs> InvitesChanged;

        event EventHandler<ChangedFriendshipSignalArgs> ChangedFriendship;

        event EventHandler<ChangedGroupsSignalArgs> ChangedGroups;

        event EventHandler<ChangedTeamsSignalArgs> ChangedTeams;

        event EventHandler<ChangedProfileSignalArgs> ChangedProfile;
    }
}
