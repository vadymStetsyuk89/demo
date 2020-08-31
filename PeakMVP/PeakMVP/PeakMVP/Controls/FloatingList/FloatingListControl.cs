using Microsoft.AppCenter.Crashes;
using PeakMVP.Controls.Stacklist.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.Controls.FloatingList {
    public class FloatingListControl : AbsoluteLayout {

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IList),
            typeof(FloatingListControl),
            defaultValue: null,
            propertyChanged: (bindable, oldValue, newValue) => {
                FloatingListControl floatingListControl = bindable as FloatingListControl;

                if (floatingListControl != null) {
                    floatingListControl.OnItemsSource();

                    if (newValue != null && newValue is INotifyCollectionChanged) {
                        ((INotifyCollectionChanged)newValue).CollectionChanged += floatingListControl.OnItemsSourceCollectionChanged;
                    }

                    if (oldValue != null && oldValue is INotifyCollectionChanged) {
                        ((INotifyCollectionChanged)oldValue).CollectionChanged -= floatingListControl.OnItemsSourceCollectionChanged;
                    }
                }
            });

        public static readonly BindableProperty ItemsTemplateProperty = BindableProperty.Create(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(FloatingListControl),
            defaultValue: null);

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            nameof(SelectedItem),
            typeof(object),
            typeof(FloatingListControl),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) => {
                FloatingListControl floatingListControl = bindable as FloatingListControl;

                if (floatingListControl != null) {
                    if (newValue != null) {
                        SourceItemBase visualSourceToSelect = floatingListControl.Children.OfType<SourceItemBase>().FirstOrDefault(i => i.BindingContext == newValue);

                        floatingListControl.OnVisualItemSelected(visualSourceToSelect);
                    }
                    else {
                        floatingListControl.Children.OfType<SourceItemBase>().ForEach(c => c.Deselected());
                    }
                }
            });

        private SourceItemBase _selectedVisualSourceItemBase;
        private double _childrenTotalYSpace = -1;

        public IList ItemsSource {
            get => (IList)GetValue(FloatingListControl.ItemsSourceProperty);
            set => SetValue(FloatingListControl.ItemsSourceProperty, value);
        }

        public object SelectedItem {
            get => GetValue(FloatingListControl.SelectedItemProperty);
            set => SetValue(FloatingListControl.SelectedItemProperty, value);
        }

        public DataTemplate ItemTemplate { get; set; }

        public double Spacing { get; set; } = 6;

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint) {
            SizeRequest sizeRequest = base.OnMeasure(widthConstraint, heightConstraint);

            if (_childrenTotalYSpace > 0) {
                sizeRequest = new SizeRequest(new Size(sizeRequest.Request.Width, _childrenTotalYSpace), sizeRequest.Minimum);
            }

            return sizeRequest;
        }

        protected override void LayoutChildren(double x, double y, double width, double height) {
            base.LayoutChildren(x, y, width, height);

            double targetX = x;
            double targetY = y;
            int currentRowItemsCount = 0;

            foreach (View child in Children) {
                double neededXSpace = child.Width + Spacing;

                if (width - targetX <= neededXSpace) {
                    targetX = 0;

                    if (currentRowItemsCount > 0) {
                        targetY += child.Height + Spacing;
                    }
                }

                LayoutChildIntoBoundingRegion(child, new Rectangle(targetX, targetY, child.Width, child.Height));

                currentRowItemsCount++;

                targetX += neededXSpace;

                if (child == Children.Last()) {
                    targetY += child.Height;
                }
            }

            _childrenTotalYSpace = targetY;

            InvalidateMeasure();
        }

        private void OnItemsSource() {
            if (ItemsSource == null || ItemTemplate == null) {
                ClearAllVisualChildren();
                return;
            }

            foreach (object item in ItemsSource) {
                Children.Add(PrepareSingleVisualChild(item));
            }
        }

        private SourceItemBase PrepareSingleVisualChild(object contextItem) {
            SourceItemBase child = null;

            try {
                child = (ItemTemplate is DataTemplate)
                    ? (ItemTemplate is DataTemplateSelector)
                        ? (SourceItemBase)((DataTemplateSelector)ItemTemplate).SelectTemplate(contextItem, this).CreateContent()
                        : (SourceItemBase)ItemTemplate.CreateContent()
                    : throw new InvalidOperationException("Can't resolve item template");

                child.SelectionAction = OnVisualItemSelected;
                child.BindingContext = contextItem;
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                throw new InvalidOperationException(
                    string.Format("FloatingListControl PrepareSingleVisualChild(object contextItem) throws exception. {0}", exc.Message));
            }

            return child;
        }

        private void OnVisualItemSelected(SourceItemBase sourceItemBaseToSelectet) {
            if (sourceItemBaseToSelectet != null) {
                _selectedVisualSourceItemBase?.Deselected();

                _selectedVisualSourceItemBase = sourceItemBaseToSelectet;
                _selectedVisualSourceItemBase.Selected();

                Children.OfType<SourceItemBase>().ForEach(vIB => {
                    if (vIB != _selectedVisualSourceItemBase) {
                        vIB.Deselected();
                    }
                    else {
                        vIB.Selected();
                    }
                });

                SelectedItem = sourceItemBaseToSelectet.BindingContext;
            }
            else {
                Children.OfType<SourceItemBase>().ForEach(vIB => vIB.Deselected());

                _selectedVisualSourceItemBase = null;
            }
        }

        private void ClearAllVisualChildren() {
            OnVisualItemSelected(null);

            Device.BeginInvokeOnMainThread(() => Children.Clear());
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            try {
                switch (e.Action) {
                    case NotifyCollectionChangedAction.Add:
                        foreach (object newItem in e.NewItems) {
                            Device.BeginInvokeOnMainThread(() => Children.Add(PrepareSingleVisualChild(newItem)));
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (object itemToRemove in e.OldItems) {
                            SourceItemBase childToRemove = Children.OfType<SourceItemBase>().FirstOrDefault(v => v.BindingContext == itemToRemove);
                            Device.BeginInvokeOnMainThread(() => Children.Remove(childToRemove));
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        ClearAllVisualChildren();
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("FloatingListControl OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) - unhandled collection changed action: {0}", e.Action));
                }
            }
            catch (Exception exc) {
                throw new InvalidOperationException(string.Format("FloatingListControl OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) throws exception. {0}", exc.Message));
            }
        }
    }
}
