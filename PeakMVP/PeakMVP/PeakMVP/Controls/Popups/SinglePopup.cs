using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.Controls.Popups {
    public class SinglePopup : ContentView {

        public static readonly BindableProperty IsViewableProperty = BindableProperty.Create(
            nameof(IsViewable),
            typeof(bool),
            typeof(SinglePopup),
            defaultValue: default(bool),
            propertyChanged: (BindableObject bindable, object oldValue, object newValue) => (bindable as SinglePopup)?.OnIsViewable());

        public static readonly BindableProperty CloseCommandProperty = BindableProperty.Create(
            nameof(CloseCommand),
            typeof(ICommand),
            typeof(SinglePopup),
            defaultValue: null);

        public event EventHandler Viewable = delegate { };

        public bool IsViewable {
            get => (bool)GetValue(SinglePopup.IsViewableProperty);
            set => SetValue(SinglePopup.IsViewableProperty, value);
        }

        public ICommand CloseCommand {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        private void OnIsViewable() {
            Viewable.Invoke(this, new EventArgs());
        }
    }
}