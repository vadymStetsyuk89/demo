using PeakMVP.Controls.Stacklist.Base;
using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Views.CompoundedViews;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace PeakMVP.Controls {
    public class WrapLayout : Layout<View> {
        private Dictionary<Size, LayoutData> layoutDataCache = new Dictionary<Size, LayoutData>();

        public static readonly BindableProperty ColumnSpacingProperty =
            BindableProperty.Create(
              "ColumnSpacing",
              typeof(double),
              typeof(WrapLayout),
              5.0,
              propertyChanged: (bindable, oldvalue, newvalue) => {
                  ((WrapLayout)bindable).InvalidateLayout();
              });
        public double ColumnSpacing {
            get => (double)GetValue(ColumnSpacingProperty);
            set => SetValue(ColumnSpacingProperty, value);
        }

        public static readonly BindableProperty RowSpacingProperty =
            BindableProperty.Create(
              "RowSpacing",
              typeof(double),
              typeof(WrapLayout),
              5.0,
              propertyChanged: (bindable, oldvalue, newvalue) => {
                  ((WrapLayout)bindable).InvalidateLayout();
              });
        public double RowSpacing {
            get => (double)GetValue(RowSpacingProperty);
            set => SetValue(RowSpacingProperty, value);
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create("ItemsSource",
                typeof(IList),
                typeof(WrapLayout),
                propertyChanged: OnItemsSourcePropertyChanged);
        public IList ItemsSource {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create("SelectedItem",
                typeof(object),
                typeof(WrapLayout),
                propertyChanged: SelectedItemPropertyChanged);
        public object SelectedItem {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        /// <summary>
        ///     ctor().
        /// </summary>
        public WrapLayout() {
        }

        private static void SelectedItemPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
            WrapLayout wrapLayout = bindable as WrapLayout;

            foreach (var item in wrapLayout.Children) {
                SourceItemBase sourceItemBase = item as SourceItemBase;
                if (sourceItemBase is SourceItemBase) {
                    sourceItemBase.Deselected();
                }
            }
        }

        private static void OnItemsSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue) {
            WrapLayout wrapLayout = bindable as WrapLayout;

            IList list = newValue as IList;
            for (int i = 0; i < list.Count; i++) {
                ProfileTileView profileTileView = new ProfileTileView {
                    BindingContext = list[i]
                };
                var item = wrapLayout.PrepareItem(profileTileView);
                wrapLayout.Children.Add(item);
            }
        }

        private SourceItemBase PrepareItem(object item) {
            SourceItemBase sourceItemBase = item as SourceItemBase;
            sourceItemBase.SelectionAction = OnItemSelected;
            return sourceItemBase;
        }

        private void OnItemSelected(SourceItemBase sourceItem) {
            if (SelectedItem is ProfileTypeItem selectedItem) {
                if (selectedItem.Equals(sourceItem.BindingContext)) {
                    SelectedItem = null;
                    sourceItem.Deselected();
                }
                else {
                    SelectedItem = sourceItem.BindingContext;
                    sourceItem.Selected();
                }
            }
            else {
                SelectedItem = sourceItem.BindingContext;
                sourceItem.Selected();
            }
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint) {
            LayoutData layoutData = GetLayoutData(widthConstraint, heightConstraint);
            if (layoutData.VisibleChildCount == 0) {
                return new SizeRequest();
            }

            Size totalSize = new Size(layoutData.CellSize.Width * layoutData.Columns + ColumnSpacing * (layoutData.Columns - 1),
                          layoutData.CellSize.Height * layoutData.Rows + RowSpacing * (layoutData.Rows - 1));
            return new SizeRequest(totalSize);
        }

        protected override void LayoutChildren(double x, double y, double width, double height) {
            LayoutData layoutData = GetLayoutData(width, height);

            if (layoutData.VisibleChildCount == 0) {
                return;
            }

            double xChild = x;
            double yChild = y;
            int row = 0;
            int column = 0;

            foreach (View child in Children) {
                if (!child.IsVisible) {
                    continue;
                }

                LayoutChildIntoBoundingRegion(child, new Rectangle(new Point(xChild, yChild), layoutData.CellSize));
                if (++column == layoutData.Columns) {
                    column = 0;
                    row++;
                    xChild = x;
                    yChild += RowSpacing + layoutData.CellSize.Height;
                }
                else {
                    xChild += ColumnSpacing + layoutData.CellSize.Width;
                }
            }
        }

        private LayoutData GetLayoutData(double width, double height) {
            Size size = new Size(width, height);

            // Check if cached information is available.
            if (layoutDataCache.ContainsKey(size)) {
                return layoutDataCache[size];
            }

            int visibleChildCount = 0;
            Size maxChildSize = new Size();
            int rows = 0;
            int columns = 0;
            LayoutData layoutData = new LayoutData();

            // Enumerate through all the children.
            foreach (View child in Children) {
                // Skip invisible children.
                if (!child.IsVisible)
                    continue;

                // Count the visible children.
                visibleChildCount++;

                // Get the child's requested size.
                SizeRequest childSizeRequest = child.Measure(Double.PositiveInfinity, Double.PositiveInfinity);

                // Accumulate the maximum child size.
                maxChildSize.Width = Math.Max(maxChildSize.Width, childSizeRequest.Request.Width);
                maxChildSize.Height = Math.Max(maxChildSize.Height, childSizeRequest.Request.Height);
            }

            if (visibleChildCount != 0) {
                // Calculate the number of rows and columns.
                if (Double.IsPositiveInfinity(width)) {
                    columns = visibleChildCount;
                    rows = 1;
                }
                else {
                    columns = (int)((width + ColumnSpacing) / (maxChildSize.Width + ColumnSpacing));
                    columns = Math.Max(1, columns);
                    rows = (visibleChildCount + columns - 1) / columns;
                }

                // Now maximize the cell size based on the layout size.
                Size cellSize = new Size();

                if (Double.IsPositiveInfinity(width))
                    cellSize.Width = maxChildSize.Width;
                else
                    cellSize.Width = (width - ColumnSpacing * (columns - 1)) / columns;

                if (Double.IsPositiveInfinity(height))
                    cellSize.Height = maxChildSize.Height;
                else
                    cellSize.Height = (height - RowSpacing * (rows - 1)) / rows;

                layoutData = new LayoutData(visibleChildCount, cellSize, rows, columns);
            }

            layoutDataCache.Add(size, layoutData);
            return layoutData;
        }

        protected override void InvalidateLayout() {
            base.InvalidateLayout();
            layoutDataCache.Clear();
        }

        protected override void OnChildMeasureInvalidated() {
            base.OnChildMeasureInvalidated();
            layoutDataCache.Clear();
        }
    }
}
