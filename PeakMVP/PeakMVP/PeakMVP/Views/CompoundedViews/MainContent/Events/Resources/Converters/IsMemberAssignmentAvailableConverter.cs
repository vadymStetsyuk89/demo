using PeakMVP.Models.DataItems.MainContent.EventsGamesSchedule;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace PeakMVP.Views.CompoundedViews.MainContent.Events.Resources.Converters {
    public class IsMemberAssignmentAvailableConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is AssignmentStatusDataItem assignmentStatusData && assignmentStatusData.Status == AssignmentStatusDataItem.ASSIGNED_STATUS_VALUE) {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new InvalidOperationException("IsMemberAssignmentAvailableConverter.ConvertBack");
        }
    }
}
