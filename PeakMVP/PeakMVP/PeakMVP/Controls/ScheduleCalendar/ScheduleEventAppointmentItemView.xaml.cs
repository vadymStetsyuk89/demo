using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Controls.ScheduleCalendar
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ScheduleEventAppointmentItemView : ContentView
	{
		public ScheduleEventAppointmentItemView ()
		{
			InitializeComponent ();
		}
	}
}