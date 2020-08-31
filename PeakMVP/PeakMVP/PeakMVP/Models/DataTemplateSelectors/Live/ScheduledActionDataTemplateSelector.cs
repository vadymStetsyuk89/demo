using PeakMVP.ViewModels.MainContent.Live;
using PeakMVP.Views.CompoundedViews.MainContent.Live.ScheduledActionItems;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace PeakMVP.Models.DataTemplateSelectors.Live {
    public class ScheduledActionDataTemplateSelector : DataTemplateSelector {

        private DataTemplate _scheduledGameTemplate = new DataTemplate(typeof(ScheduledGameViewCell));
        private DataTemplate _scheduledCompletedGameTemplate = new DataTemplate(typeof(ScheduledCompletedGameViewCell));
        private DataTemplate _scheduledEventTemplate = new DataTemplate(typeof(ScheduledEventViewCell));

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container) {
            if (item is ScheduledGame scheduledGame) {
                return scheduledGame.IsCompleted ? _scheduledCompletedGameTemplate : _scheduledGameTemplate;
            }
            else if (item is ScheduledEvent) {
                return _scheduledEventTemplate;
            }
            else {
                Debugger.Break();
                throw new InvalidOperationException("ScheduledActionDataTemplateSelector unresolved type");
            }
        }
    }
}
