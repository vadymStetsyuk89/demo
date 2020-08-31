using PeakMVP.Controls.Popovers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.CompoundedViews.MainContent.Groups {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GroupTypePopover : PopoverBase {
        public GroupTypePopover() {
            InitializeComponent();
        }
    }
}