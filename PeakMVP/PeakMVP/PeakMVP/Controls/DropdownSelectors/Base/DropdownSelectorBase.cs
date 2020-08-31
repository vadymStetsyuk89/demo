using Microsoft.AppCenter.Crashes;
using PeakMVP.Controls.Popovers;
using PeakMVP.Controls.Popovers.Arguments;
using PeakMVP.Controls.Popovers.Base;
using System;
using System.Collections;
using System.Collections.Specialized;
using Xamarin.Forms;

namespace PeakMVP.Controls.DropdownSelectors.Base {
    public abstract class DropdownSelectorBase : ContentView {

        private static readonly Color _DEFAULT_BORDER_COLOR = Color.Transparent;
        protected static readonly string _BORDER_COLOR_PROPERTY_PATH = nameof(BorderColor);

        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create(nameof(BorderColor),
                typeof(Color),
                typeof(DropdownSelectorBase),
                defaultValue: DropdownSelectorBase._DEFAULT_BORDER_COLOR);

        public static readonly BindableProperty PopoverProperty =
            BindableProperty.Create(nameof(Popover),
                typeof(IPopover),
                typeof(DropdownSelectorBase),
                defaultValue: null,
                propertyChanged: (bindable, oldValue, newValue) => {
                    DropdownSelectorBase dropdownSelectorBase = bindable as DropdownSelectorBase;

                    if (dropdownSelectorBase != null) {
                        dropdownSelectorBase.DisposePopover(oldValue as IPopover);
                        dropdownSelectorBase.WiringUpPopoverEssense();
                        dropdownSelectorBase.OnPopoverChanged();
                    }
                });

        public static readonly BindableProperty ItemSourceProperty =
            BindableProperty.Create(nameof(ItemSource),
                typeof(IList),
                typeof(DropdownSelectorBase),
                defaultValue: null,
                propertyChanged: (bindable, oldValue, newValue) => (bindable as DropdownSelectorBase)?.ApplyItemSource((IList)oldValue, (IList)newValue));

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem),
                typeof(object),
                typeof(DropdownSelectorBase),
                defaultValue: null,
                propertyChanged: (bindable, oldValue, newValue) => (bindable as DropdownSelectorBase)?.OnItemSelected());

        public static readonly BindableProperty IsPopoverVisibleProperty =
            BindableProperty.Create(
                nameof(IsPopoverVisible),
                typeof(bool),
                typeof(DropdownSelectorBase),
                defaultValue: default(bool),
                defaultBindingMode: BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) => (bindable as DropdownSelectorBase)?.OnIsPopoverVisible());

        public static readonly BindableProperty PopoverLayoutingProperty =
            BindableProperty.Create(nameof(PopoverLayouting),
                typeof(PopoverLayoutingStrategy),
                typeof(DropdownSelectorBase),
                defaultValue: default(PopoverLayoutingStrategy));

        private IPopoverKeeper PopoverKeeper => FindPopoverKeeper();

        public DropdownSelectorBase() { }

        public bool IsPopoverVisible {
            get => (bool)GetValue(DropdownSelectorBase.IsPopoverVisibleProperty);
            set => SetValue(DropdownSelectorBase.IsPopoverVisibleProperty, value);
        }

        public Color BorderColor {
            get => (Color)GetValue(DropdownSelectorBase.BorderColorProperty);
            set => SetValue(DropdownSelectorBase.BorderColorProperty, value);
        }

        public IPopover Popover {
            get => (IPopover)GetValue(DropdownSelectorBase.PopoverProperty);
            set => SetValue(DropdownSelectorBase.PopoverProperty, value);
        }

        public IList ItemSource {
            get => (IList)GetValue(DropdownSelectorBase.ItemSourceProperty);
            set => SetValue(DropdownSelectorBase.ItemSourceProperty, value);
        }

        public object SelectedItem {
            get => GetValue(DropdownSelectorBase.SelectedItemProperty);
            set => SetValue(DropdownSelectorBase.SelectedItemProperty, value);
        }

        public PopoverLayoutingStrategy PopoverLayouting {
            get => (PopoverLayoutingStrategy)GetValue(PopoverLayoutingProperty);
            set => SetValue(PopoverLayoutingProperty, value);
        }

        protected virtual void OnPopoverChanged() { }

        protected virtual void OnItemSelected() =>
            IsPopoverVisible = false;

        protected virtual void OnItemSourceApplayed() {
            ProbeItemSourceForPopoverViewing();
        }

        protected void HidePopover() =>
            PopoverKeeper?.HideAllPopovers();

        protected void ShowPopover() {
            Point popoverDestinationPoint = new Point(X, (Y));

            VisualElement parent = (VisualElement)Parent;
            ScrollView extraScrollView = null;
            ListViewExtended extraListViewExtended = null;

            try {
                while (parent != null && !(parent is IPopoverKeeper)) {
                    if (parent is ScrollView) {
                        extraScrollView = (ScrollView)parent;
                    }
                    else if (parent is ListViewExtended) {
                        extraListViewExtended = (ListViewExtended)parent;
                    }

                    popoverDestinationPoint.X += parent.X;
                    popoverDestinationPoint.Y += parent.Y;

                    parent = (VisualElement)parent.Parent;
                }
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);
                parent = null;
            }

            if (extraScrollView != null) {
                popoverDestinationPoint.Y = popoverDestinationPoint.Y - extraScrollView.ScrollY;
            }

            if (extraListViewExtended != null) {
                popoverDestinationPoint.Y = popoverDestinationPoint.Y - extraListViewExtended.YScrollOutput;
            }

            if (parent != null && parent is IPopoverKeeper) {
                ((IPopoverKeeper)parent).ShowPopover(Popover, new ShowPopoverArgs() {
                    PopoverLayoutingStrategy = PopoverLayouting,
                    DropDownSelectrorRectangle = new Rectangle(popoverDestinationPoint, new Size(Width, Height))
                });
            }
        }

        private void DisposePopover(IPopover targetPopover) {
            if (targetPopover != null) {
                IPopoverKeeper popoverKeeper = FindPopoverKeeper();
                popoverKeeper?.HideAllPopovers();

                targetPopover.ItemContext = null;
                targetPopover.SelectedItem = null;

                ((BindableObject)targetPopover).RemoveBinding(BindableObject.BindingContextProperty);
                ((BindableObject)targetPopover).RemoveBinding(PopoverBase.IsPopoverVisibleProperty);
            }
        }

        private IPopoverKeeper FindPopoverKeeper() {
            VisualElement parent = (VisualElement)Parent;

            while (parent != null && !(parent is IPopoverKeeper)) {
                parent = (VisualElement)parent.Parent;
            }

            return (parent != null && parent is IPopoverKeeper) ? (IPopoverKeeper)parent : null;
        }

        private void WiringUpPopoverEssense() {
            if (Popover != null) {
                ((BindableObject)Popover).SetBinding(PopoverBase.ItemContextProperty, new Binding(nameof(ItemSource), mode: BindingMode.OneWay, source: this));
                ((BindableObject)Popover).SetBinding(PopoverBase.SelectedItemProperty, new Binding(nameof(SelectedItem), mode: BindingMode.TwoWay, source: this));
                ((BindableObject)Popover).SetBinding(BindableObject.BindingContextProperty, new Binding(nameof(BindingContext), mode: BindingMode.OneWay, source: this));
                ((BindableObject)Popover).SetBinding(PopoverBase.IsPopoverVisibleProperty, new Binding(nameof(IsPopoverVisible), mode: BindingMode.TwoWay, source: this));
            }
        }

        private void ApplyItemSource(IList oldValue, IList newValue) {
            if (oldValue is INotifyCollectionChanged) {
                ((INotifyCollectionChanged)oldValue).CollectionChanged -= OnItemSourceCollectionChanged;
            }

            if (newValue is INotifyCollectionChanged) {
                ((INotifyCollectionChanged)newValue).CollectionChanged += OnItemSourceCollectionChanged;
            }

            OnItemSourceApplayed();
        }

        private void OnItemSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) =>
            OnItemSourceApplayed();

        private void ProbeItemSourceForPopoverViewing() =>
            IsPopoverVisible = ItemSource?.Count > 0;

        private void OnIsPopoverVisible() {
            if (IsPopoverVisible) {
                ShowPopover();
            }
            else {
                HidePopover();
            }
        }
    }
}