using Microsoft.AppCenter.Crashes;
using PeakMVP.Extensions;
using PeakMVP.Helpers;
using PeakMVP.Helpers.AppEvents.Events;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Exceptions;
using PeakMVP.Models.Identities.Scheduling;
using PeakMVP.Models.Rests.DTOs;
using PeakMVP.Models.Rests.DTOs.TeamMembership;
using PeakMVP.Services.Scheduling;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.MainContent.Events.Arguments;
using PeakMVP.Views.CompoundedViews.MainContent.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Events {
    public class ListViewingOfEventsAndGamesContentViewModel : NestedViewModel, IVisualFiguring, IEventManagementDependent {

        private readonly ISchedulingService _schedulingService;

        private CancellationTokenSource _deleteGameActionCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _deleteEventActionCancellationTokenSource = new CancellationTokenSource();

        private TeamMember _targetTeamMember;

        public ListViewingOfEventsAndGamesContentViewModel(
            ISchedulingService schedulingService) {
            _schedulingService = schedulingService;
        }

        public ICommand EditGameEventCommand => new Command(async (object args) => {
            ContentPageBaseViewModel relativeBasePage = NavigationService.LastPageViewModel as ContentPageBaseViewModel;
            Guid busyKey;

            if (relativeBasePage != null) {
                busyKey = Guid.NewGuid();
                relativeBasePage.SetBusy(busyKey, true);
                await Task.Delay(AppConsts.DELAY_STUB);
            }

            if (args is GameDTO gameAction) {
                await NavigationService.NavigateToAsync<EditGameViewModel>();
                await NavigationService.LastPageViewModel.InitializeAsync(_targetTeamMember);
                await NavigationService.LastPageViewModel.InitializeAsync(gameAction);
            }
            else if (args is EventDTO eventAction) {
                await NavigationService.NavigateToAsync<EditEventViewModel>();
                await NavigationService.LastPageViewModel.InitializeAsync(_targetTeamMember);
                await NavigationService.LastPageViewModel.InitializeAsync(eventAction);
            }
            else {
                Debugger.Break();
            }

            if (relativeBasePage != null) {
                relativeBasePage.SetBusy(busyKey, false);
            }
        });

        public ICommand DeleteScheduledActionCommand => new Command((object args) => {
            if (args is GameDTO gameAction) {
                DeleteGame(gameAction);
            }
            else if (args is EventDTO eventAction) {
                DeletEvent(eventAction);
            }
            else {
                Debugger.Break();
            }
        });

        public Type RelativeViewType => typeof(ListViewingOfEventsAndGamesContentView);

        public string TabHeader { get; private set; }

        private bool _isEventsManagementAvailable = (GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Coach || GlobalSettings.Instance.UserProfile.ProfileType == ProfileType.Organization);
        public bool IsEventsManagementAvailable {
            get => _isEventsManagementAvailable;
            private set => SetProperty<bool>(ref _isEventsManagementAvailable, value);
        }

        private ObservableCollection<ScheduleAction> _activeRepetableEvents;
        public ObservableCollection<ScheduleAction> ActiveRepetableEvents {
            get => _activeRepetableEvents;
            private set => SetProperty(ref _activeRepetableEvents, value);
        }

        private ObservableCollection<ScheduleAction> _gamesAndEvents;
        public ObservableCollection<ScheduleAction> GamesAndEvents {
            get => _gamesAndEvents;
            private set => SetProperty(ref _gamesAndEvents, value);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _deleteGameActionCancellationTokenSource);
            ResetCancellationTokenSource(ref _deleteEventActionCancellationTokenSource);
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _deleteGameActionCancellationTokenSource);
            ResetCancellationTokenSource(ref _deleteEventActionCancellationTokenSource);
        }

        public override Task InitializeAsync(object navigationData) {

            if (navigationData is ExtractEventsAndGamesForSelectedTeamArgs gamesForSelectedTeamArgs) {
                _targetTeamMember = gamesForSelectedTeamArgs.TargetTeamMember;

                GamesAndEvents = gamesForSelectedTeamArgs.TargetTeamMember.Team?.Events?
                    .Cast<TeamActionBaseDTO>()
                    .Concat(gamesForSelectedTeamArgs.TargetTeamMember.Team.Games ?? new List<GameDTO> { })
                    .Select<TeamActionBaseDTO, ScheduleAction>(tADTO => new ScheduleAction(tADTO, gamesForSelectedTeamArgs.TargetTeamMember))
                    .ToObservableCollection<ScheduleAction>();

                ActiveRepetableEvents = gamesForSelectedTeamArgs.TargetTeamMember.Team?.Events?
                    .Where<EventDTO>(eDTO => (eDTO.RepeatingType == EventDTO.DAILY_REPITITION_TYPE || eDTO.RepeatingType == EventDTO.WEEKLY_REPITITION_TYPE))
                    .Select<EventDTO, ScheduleAction>(eDTO => new ScheduleAction(eDTO, gamesForSelectedTeamArgs.TargetTeamMember))
                    .ToObservableCollection<ScheduleAction>();
            }

            return base.InitializeAsync(navigationData);
        }

        private async void DeleteGame(GameDTO gameAction) {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            ResetCancellationTokenSource(ref _deleteGameActionCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _deleteGameActionCancellationTokenSource;

            try {
                await _schedulingService.DeleteGameAsync(gameAction, _targetTeamMember.Team.Id, cancellationTokenSource);

                GameDTO gameToRemove = _targetTeamMember.Team.Games.FirstOrDefault(gDTO => gDTO.Id == gameAction.Id);
                _targetTeamMember.Team.Games.Remove(gameToRemove);

                ScheduleAction scheduleActionToRemove = GamesAndEvents.FirstOrDefault(sA => sA.ScheduledAction is GameDTO && sA.ScheduledAction.Id == gameAction.Id);
                GamesAndEvents.Remove(scheduleActionToRemove);

                GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.InvokeGameDeleted(this, new GameManagmentArgs() { Game = gameAction, TeamMember = _targetTeamMember });
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                Device.BeginInvokeOnMainThread(async () => await DialogService.ToastAsync(exc.Message));
            }

            UpdateBusyVisualState(busyKey, false);
        }

        private async void DeletEvent(EventDTO eventAction) {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            ResetCancellationTokenSource(ref _deleteEventActionCancellationTokenSource);
            CancellationTokenSource cancellationTokenSource = _deleteEventActionCancellationTokenSource;

            try {
                await _schedulingService.DeleteEventAsync(eventAction, _targetTeamMember.Team.Id, cancellationTokenSource);

                EventDTO eventToRemove = _targetTeamMember.Team.Events.FirstOrDefault(eDTO => eDTO.Id == eventAction.Id);
                _targetTeamMember.Team.Events.Remove(eventToRemove);

                ScheduleAction scheduleActionToRemove = GamesAndEvents.FirstOrDefault(sA => sA.ScheduledAction is EventDTO && sA.ScheduledAction.Id == eventAction.Id);
                GamesAndEvents.Remove(scheduleActionToRemove);

                ScheduleAction scheduleRepeteableActionToRemove = ActiveRepetableEvents.FirstOrDefault(sA => sA.ScheduledAction is EventDTO && sA.ScheduledAction.Id == eventAction.Id);
                ActiveRepetableEvents.Remove(scheduleRepeteableActionToRemove);

                GlobalSettings.Instance.AppMessagingEvents.ScheduleEvents.InvokeEventDeleted(this, new EventManagmentArgs() { Event = eventAction, TeamMember = _targetTeamMember });
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (ServiceAuthenticationException) { }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                Device.BeginInvokeOnMainThread(async () => await DialogService.ToastAsync(exc.Message));
            }

            UpdateBusyVisualState(busyKey, false);
        }
    }
}
