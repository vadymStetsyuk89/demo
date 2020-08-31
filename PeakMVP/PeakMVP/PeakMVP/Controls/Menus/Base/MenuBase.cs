using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace PeakMVP.Controls.Menus.Base {
    public abstract class MenuBase : ContentView {

        public static readonly BindableProperty MenuOptionsProperty =
            BindableProperty.Create(nameof(MenuOptions),
                typeof(IList),
                typeof(MenuBase),
                defaultValue: null);

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem),
                typeof(object),
                typeof(MenuBase),
                defaultValue: null);

        public MenuBase() { }

        public IList MenuOptions {
            get => (IList)GetValue(MenuBase.MenuOptionsProperty);
            set => SetValue(MenuBase.MenuOptionsProperty, value);
        }

        public object SelectedItem {
            get => GetValue(MenuBase.SelectedItemProperty);
            set => SetValue(MenuBase.SelectedItemProperty, value);
        }
    }
}