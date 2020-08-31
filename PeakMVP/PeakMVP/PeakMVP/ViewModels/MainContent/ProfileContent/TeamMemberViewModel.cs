using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Teams;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.ProfileContent {
    public sealed class TeamMemberViewModel : ViewModelBase {

        public ICommand OwnerOverviewCommand => new Command(async () => {
            await DialogService.ToastAsync(string.Format("Owner, {0}, overview command in developing", Owner));
        });

        public ICommand TeamOverviewCommand => new Command(async () => await NavigationService.NavigateToAsync<TeamsInfoViewModel>(Data));

        private bool _canBeDeleted;
        public bool CanBeDeleted {
            get => _canBeDeleted;
            set => SetProperty<bool>(ref _canBeDeleted, value);
        }

        private TeamMember _data;
        public TeamMember Data {
            get => _data;
            set => SetProperty(ref _data, value);
        }

        string _icon;
        public string Icon {
            get { return _icon; }
            set { SetProperty(ref _icon, value); }
        }

        string _sport;
        public string Sport {
            get { return _sport; }
            set { SetProperty(ref _sport, value); }
        }

        string _owner;
        public string Owner {
            get { return _owner; }
            set { SetProperty(ref _owner, value); }
        }

        string _team;
        public string Team {
            get { return _team; }
            set { SetProperty(ref _team, value); }
        }

        private string _role;
        public string Role {
            get => _role;
            set => SetProperty<string>(ref _role, value);
        }

        DateTime? _joined;
        public DateTime? Joined {
            get { return _joined; }
            set { SetProperty(ref _joined, value); }
        }
    }
}
