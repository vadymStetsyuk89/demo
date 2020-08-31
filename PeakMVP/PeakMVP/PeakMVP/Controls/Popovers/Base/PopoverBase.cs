using System.Collections;

using Xamarin.Forms;

namespace PeakMVP.Controls.Popovers.Base {
    public abstract class PopoverBase : ContentView, IPopover {

        public static readonly BindableProperty ItemContextProperty = BindableProperty.Create(
            nameof(ItemContext),
            typeof(IList),
            typeof(PopoverBase),
            defaultValue: null,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as PopoverBase)?.OnItemContextChanged());

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            nameof(SelectedItem),
            typeof(object),
            typeof(PopoverBase),
            defaultValue: null,
            propertyChanged: (bindable, oldValue, newValue) => (bindable as PopoverBase)?.OnSelectedItemPropertyChanged());

        public static readonly BindableProperty HintTextProperty = BindableProperty.Create(
            nameof(HintText),
            typeof(string),
            typeof(PopoverBase),
            defaultValue: null);

        public static readonly BindableProperty IsPopoverVisibleProperty = BindableProperty.Create(
            nameof(IsPopoverVisible),
            typeof(bool),
            typeof(PopoverBase),
            defaultValue: default(bool));

        public PopoverBase() { }

        public bool IsPopoverVisible {
            get => (bool)GetValue(PopoverBase.IsPopoverVisibleProperty);
            set => SetValue(PopoverBase.IsPopoverVisibleProperty, value);
        }

        public IList ItemContext {
            get => (IList)GetValue(ItemContextProperty);
            set => SetValue(ItemContextProperty, value);
        }

        public string HintText {
            get => (string)GetValue(HintTextProperty);
            set => SetValue(HintTextProperty, value);
        }

        public object SelectedItem {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public bool IsHaveSameWidth { get; set; }

        protected virtual void OnItemContextChanged() { }

        protected virtual void OnSelectedItemPropertyChanged() { }
    }
}