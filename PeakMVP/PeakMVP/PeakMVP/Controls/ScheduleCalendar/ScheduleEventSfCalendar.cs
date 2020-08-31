using Syncfusion.SfCalendar.XForms;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.Controls.ScheduleCalendar {
    public class ScheduleEventSfCalendar : SfCalendar {

        public static readonly BindableProperty ScheduleEventsSourceProperty = BindableProperty.Create(
            nameof(ScheduleEventsSource),
            typeof(IEnumerable<IScheduleEvent>),
            typeof(ScheduleEventSfCalendar),
            defaultValue: default(IEnumerable<IScheduleEvent>),
            propertyChanged: (BindableObject bindable, object oldValue, object newValue) => {
                if (bindable is ScheduleEventSfCalendar declarer) {
                    CalendarEventCollection calendarInlineEvents = new CalendarEventCollection();

                    if (oldValue is INotifyCollectionChanged oldNotifyCollection) {
                        oldNotifyCollection.CollectionChanged -= declarer.OnNewNotifyCollectionCollectionChanged;
                    }

                    if (newValue is INotifyCollectionChanged newNotifyCollection) {
                        newNotifyCollection.CollectionChanged += declarer.OnNewNotifyCollectionCollectionChanged;
                    }

                    if (newValue is IEnumerable<IScheduleEvent> newScheduleEvents) {
                        //newScheduleEvents.ForEach((oneScheduleEvent) => {
                        //    declarer.DataSource.Add(declarer.BuildScheduleCalendarInlineEvent(oneScheduleEvent));
                        //});                        
                        newScheduleEvents.ForEach((oneScheduleEvent) => {
                            calendarInlineEvents.Add(declarer.BuildScheduleCalendarInlineEvent(oneScheduleEvent));
                        });
                    }

                    declarer.DataSource = calendarInlineEvents;
                    //declarer.Refresh();
                }
            });

        public static BindableProperty CommandProperty = BindableProperty.Create(
            nameof(Command),
            typeof(ICommand),
            typeof(ScheduleEventSfCalendar),
            defaultValue: default(ICommand));

        public ScheduleEventSfCalendar() {
            //DataSource = new CalendarEventCollection();

            OnCalendarTapped += OnScheduleEventSfCalendarOnCalendarTapped;
        }

        public ICommand Command {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public IEnumerable<IScheduleEvent> ScheduleEventsSource {
            get => (IEnumerable<IScheduleEvent>)GetValue(ScheduleEventsSourceProperty);
            set => SetValue(ScheduleEventsSourceProperty, value);
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            OnCalendarTapped -= OnScheduleEventSfCalendarOnCalendarTapped;
        }

        private ScheduleCalendarInlineEvent BuildScheduleCalendarInlineEvent(IScheduleEvent eventDataModel) {
            ScheduleCalendarInlineEvent inlineEvent = null;

            if (eventDataModel != null) {
                inlineEvent = new ScheduleCalendarInlineEvent() {
                    StartTime = eventDataModel.StartDate,
                    EndTime = eventDataModel.StartDate,
                    Subject = eventDataModel.Subject,
                    Color = Color.LightSeaGreen,
                    SceduleEventContext = eventDataModel
                };
            }

            return inlineEvent;
        }

        private void OnNewNotifyCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            try {
                switch (e.Action) {
                    case NotifyCollectionChangedAction.Add:
                        Debugger.Break();
                        foreach (IScheduleEvent newItem in e.NewItems) {
                            DataSource.Add(BuildScheduleCalendarInlineEvent(newItem));
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        Debugger.Break();
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        Debugger.Break();
                        foreach (IScheduleEvent oldItem in e.OldItems) {
                            DataSource.Remove(DataSource.OfType<ScheduleCalendarInlineEvent>().FirstOrDefault<ScheduleCalendarInlineEvent>(inlineEvent => inlineEvent.SceduleEventContext == oldItem));
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        Debugger.Break();
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        DataSource.Clear();
                        break;
                    default:
                        Debugger.Break();
                        break;
                }
            }
            catch (System.Exception) {
                Debugger.Break();
                throw;
            }
        }

        private List<IScheduleEvent> GetSelectedScheduledActions(DateTime date) {
            List<IScheduleEvent> selectedDateAppointments = new List<IScheduleEvent>();

            for (int i = 0; i < ScheduleEventsSource.Count(); i++) {
                DateTime startDate = ScheduleEventsSource.ElementAt<IScheduleEvent>(i).StartDate;

                if (date.Day == startDate.Day && date.Month == startDate.Month && date.Year == startDate.Year) {
                    selectedDateAppointments.Add(ScheduleEventsSource.ElementAt<IScheduleEvent>(i));
                }
            }

            return selectedDateAppointments;
        }

        private void OnScheduleEventSfCalendarOnCalendarTapped(object sender, CalendarTappedEventArgs args) {
            if (Command != null) {
                Command.Execute(SelectedDate);
            }
        }
    }
}