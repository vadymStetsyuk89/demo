using Microsoft.AppCenter.Crashes;
using PeakMVP.Factories.Validation;
using PeakMVP.Models.Exceptions;
using PeakMVP.Validations;
using PeakMVP.ViewModels.Base;
using PeakMVP.ViewModels.Base.Popups;
using PeakMVP.Views.CompoundedViews.MainContent.Live.ScheduleEventInfo.Popups;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.MainContent.Live.ScheduledEventInfo.Popups {
    public class AddAvailabilityNotePopupViewModel : PopupBaseViewModel, IInputForm {

        private readonly IValidationObjectFactory _validationObjectFactory;

        private CancellationTokenSource _changeAvailabilityNoteCancellationTokenSource = new CancellationTokenSource();

        private ScheduledEventBase _scheduledEvent;

        public AddAvailabilityNotePopupViewModel(
            IValidationObjectFactory validationObjectFactory) {

            _validationObjectFactory = validationObjectFactory;

            ResetInputForm();
        }

        public ICommand AddAvailabilityNoteCommand => new Command(async () => {
            Guid busyKey = Guid.NewGuid();
            UpdateBusyVisualState(busyKey, true);

            if (ValidateForm()) {
                ResetCancellationTokenSource(ref _changeAvailabilityNoteCancellationTokenSource);
                CancellationTokenSource cancellationTokenSource = _changeAvailabilityNoteCancellationTokenSource;

                try {
                    ///
                    /// TODO: use appropriate api to update availability note
                    /// 
                    ClosePopupCommand.Execute(null);

                    _scheduledEvent.AvailabilityNote = AvailabilityNote.Value;
                    GlobalSettings.Instance.AppMessagingEvents.LiveScheduleEvents.ScheduledEventUpdatedInvoke(this, _scheduledEvent);
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
                catch (ServiceAuthenticationException) { }
                catch (Exception exc) {
                    Crashes.TrackError(exc);

                    await DialogService.ToastAsync(exc.Message);
                }
            }

            UpdateBusyVisualState(busyKey, false);
        });

        public override Type RelativeViewType => typeof(AddAvailabilityNotePopupView);

        private ValidatableObject<string> _availabilityNote;
        public ValidatableObject<string> AvailabilityNote {
            get => _availabilityNote;
            set => SetProperty<ValidatableObject<string>>(ref _availabilityNote, value);
        }

        public override void Dispose() {
            base.Dispose();

            ResetCancellationTokenSource(ref _changeAvailabilityNoteCancellationTokenSource);
        }

        public void ResetInputForm() {
            _scheduledEvent = null;

            ResetValidationObjects();
        }

        public bool ValidateForm() {
            bool validationResult = false;

            validationResult = AvailabilityNote.Validate();

            return validationResult;
        }

        protected override void OnIsPopupVisible() {
            base.OnIsPopupVisible();

            if (!IsPopupVisible) {
                ResetInputForm();
            }
        }

        protected override void LoseIntent() {
            base.LoseIntent();

            ResetCancellationTokenSource(ref _changeAvailabilityNoteCancellationTokenSource);
        }

        protected override void OnShowPopupCommand(object param) {
            base.OnShowPopupCommand(param);

            if (param is ScheduledEventBase scheduledEvent) {
                _scheduledEvent = scheduledEvent;
                AvailabilityNote.Value = scheduledEvent.AvailabilityNote;
            }
        }

        private void ResetValidationObjects() {
            AvailabilityNote = _validationObjectFactory.GetValidatableObject<string>();
        }
    }
}
