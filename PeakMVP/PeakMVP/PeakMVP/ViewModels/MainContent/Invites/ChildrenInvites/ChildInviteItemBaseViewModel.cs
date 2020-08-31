using PeakMVP.Models.Rests.DTOs;
using PeakMVP.ViewModels.Base;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Invites.ChildrenInvites {
    public abstract class ChildInviteItemBaseViewModel : NestedViewModel {

        public ICommand AcceptCommand { get; protected set; }

        public ICommand DeclineCommand { get; protected set; }

        public IInviteTo InviteTo { get; private set; }

        public ProfileDTO Child { get; private set; }

        string _companionScopeTitle;
        public string CompanionScopeTitle {
            get => _companionScopeTitle;
            protected set => SetProperty<string>(ref _companionScopeTitle, value);
        }

        string _companionAvatarPath;
        public string CompanionAvatarPath {
            get => _companionAvatarPath;
            protected set => SetProperty<string>(ref _companionAvatarPath, value);
        }

        string _mainInviteDescription;
        public string MainInviteDescription {
            get => _mainInviteDescription;
            protected set => SetProperty<string>(ref _mainInviteDescription, value);
        }

        bool _isAvatarEnabled;
        public bool IsAvatarEnabled {
            get => _isAvatarEnabled;
            protected set => SetProperty<bool>(ref _isAvatarEnabled, value);
        }

        public void UploadParticipants(IInviteTo inviteTo, ProfileDTO child) {
            if (inviteTo != null && child != null) {
                InviteTo = inviteTo;
                Child = child;

                OnUploadParticipants();
            }
            else {
                Debugger.Break();
                throw new InvalidOperationException("ChildInviteItemBaseViewModel.UploadParticipants can't upload participants");
            }
        }

        protected abstract void OnUploadParticipants();
    }
}
