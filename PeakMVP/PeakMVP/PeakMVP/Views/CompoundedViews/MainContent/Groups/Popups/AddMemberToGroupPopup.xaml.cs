using PeakMVP.Controls.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.CompoundedViews.MainContent.Groups.Popups {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddMemberToGroupPopup : SinglePopup {
        public AddMemberToGroupPopup() {
            InitializeComponent();
        }
    }
}