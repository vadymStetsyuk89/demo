using PeakMVP.Views.Base;
using System;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.Authorization {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginView : ContentPageBase {
        public LoginView() {
            InitializeComponent();
        }
    }
}