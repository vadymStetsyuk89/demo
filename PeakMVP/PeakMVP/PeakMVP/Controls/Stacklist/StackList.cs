using Microsoft.AppCenter.Crashes;
using PeakMVP.Controls.Stacklist.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.Controls.Stacklist {
    public class StackList : StackLayout {

        private static readonly string _ERORR_LAYOUT_FILLING = "Check validity of ItemsSource or ItemTemplate";

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IList),
            typeof(StackList),
            propertyChanged: (bindable, oldValue, newValue) => {
                StackList layout = bindable as StackList;

                if (layout.ItemTemplate != null) {
                    layout.FillLayout();

                    if (oldValue != null && oldValue is INotifyCollectionChanged) {
                        ((INotifyCollectionChanged)oldValue).CollectionChanged -= layout.StackList_CollectionChanged;
                    }

                    if (newValue != null && newValue is INotifyCollectionChanged) {
                        ((INotifyCollectionChanged)newValue).CollectionChanged += layout.StackList_CollectionChanged;
                    }
                }
            });

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(StackList),
            propertyChanged: (bindable, oldValue, newValue) => {
                StackList layout = bindable as StackList;

                if (layout != null && layout.ItemsSource != null) {
                    layout.FillLayout();
                }
            });

        public static readonly BindableProperty GroupHeaderTemplateProperty = BindableProperty.Create(
            nameof(GroupHeaderTemplate),
            typeof(DataTemplate),
            typeof(StackList),
            propertyChanged: (bindable, oldValue, newValue) => {
                StackList layout = bindable as StackList;

                if (layout != null && layout.ItemsSource != null) {
                    layout.FillLayout();
                }
            });

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            nameof(SelectedItem),
            typeof(object),
            typeof(StackList),
            propertyChanged: (bindable, oldValue, newValue) => {
                StackList stackList = bindable as StackList;

                if (stackList != null) {
                    if (newValue != null) {
                        SourceItemBase selectedStackListItemBase = stackList._stackListSingleItemsChildren
                                                                .FirstOrDefault(i => i.BindingContext == newValue);

                        stackList._stackListSingleItemsChildren.Except(new[] { selectedStackListItemBase }).ForEach(c => c.Deselected());

                        if (selectedStackListItemBase != null) {
                            stackList._selectedStackListItem = selectedStackListItemBase;
                            stackList._selectedStackListItem.Selected();
                        }
                    }
                    else {
                        stackList._stackListSingleItemsChildren.ForEach(c => c.Deselected());
                    }
                }
            });

        private List<SourceItemBase> _stackListSingleItemsChildren = new List<SourceItemBase>();
        private SourceItemBase _selectedStackListItem;
        public bool IsMultiRowEnabled { get; set; }
        public int ItemsPerRow { get; set; } = 1;
        public LayoutOptions RowHorizontalLayoutOptions { get; set; }
        public MultiRowStrategy MultiRowStrategy { get; set; }

        public StackList() {
            Spacing = 0;
            IsGrouped = false;
        }

        public IList ItemsSource {
            get => (IList)GetValue(StackList.ItemsSourceProperty);
            set => SetValue(StackList.ItemsSourceProperty, value);
        }

        public DataTemplate ItemTemplate {
            get => (DataTemplate)GetValue(StackList.ItemTemplateProperty);
            set => SetValue(StackList.ItemTemplateProperty, value);
        }

        public DataTemplate GroupHeaderTemplate {
            get => (DataTemplate)GetValue(StackList.GroupHeaderTemplateProperty);
            set => SetValue(StackList.GroupHeaderTemplateProperty, value);
        }

        public object SelectedItem {
            get => GetValue(StackList.SelectedItemProperty);
            set => SetValue(StackList.SelectedItemProperty, value);
        }

        public bool StretchLastOddElement { get; set; }

        public bool IsGrouped { get; set; }

        private void StackList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            try {
                if (e.Action == NotifyCollectionChangedAction.Add) {
                    if (IsGrouped) {
                        foreach (object group in e.NewItems) {
                            Children.Add(PrepareGroup(group));
                        }
                    }
                    else {
                        if (IsMultiRowEnabled) {
                            Layout<View> rowStack = Children.OfType<Layout<View>>().LastOrDefault();

                            switch (MultiRowStrategy) {
                                case MultiRowStrategy.Stack:
                                    if (rowStack == null || rowStack.Children.Count == ItemsPerRow) {
                                        rowStack = BuildStackRow();
                                    }

                                    foreach (object item in e.NewItems) {
                                        SourceItemBase singleItem = PrepareSingleItem(item);
                                        rowStack.Children.Add(singleItem);

                                        if (rowStack.Children.Count == ItemsPerRow) {
                                            if (rowStack.Parent == null) {
                                                Children.Add(rowStack);
                                            }
                                            rowStack = BuildStackRow();
                                        }
                                    }
                                    break;
                                case MultiRowStrategy.Grid:
                                    if (rowStack == null) {
                                        rowStack = BuildGridRow(ItemsPerRow);
                                    }

                                    for (int i = 0; i < e.NewItems.Count; i++) {
                                        SourceItemBase singleItem = PrepareSingleItem(e.NewItems[i]);
                                        rowStack.Children.Add(singleItem);

                                        int totalIndex = ItemsSource.IndexOf(e.NewItems[i]);
                                        int row = totalIndex / ItemsPerRow;

                                        if (((Grid)rowStack).RowDefinitions.Count < (row + 1)) {
                                            RowDefinition rowDefinition = new RowDefinition();
                                            rowDefinition.Height = GridLength.Auto;

                                            ((Grid)rowStack).RowDefinitions.Add(rowDefinition);
                                        }

                                        Grid.SetColumn(singleItem, totalIndex % ItemsPerRow);
                                        if (StretchLastOddElement) {
                                            if (i == ItemsSource.Count - 1 && (i % ItemsPerRow) == 0) {
                                                Grid.SetColumnSpan(singleItem, ItemsPerRow);
                                            }
                                        }
                                        Grid.SetRow(singleItem, row);
                                    }
                                    break;
                                default:
                                    break;
                            }

                            if (rowStack.Children.Any() && rowStack.Parent == null) {
                                Children.Add(rowStack);
                            }
                        }
                        else {
                            foreach (object item in e.NewItems) {
                                Children.Add(PrepareSingleItem(item));
                            }
                        }
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove) {
                    if (IsMultiRowEnabled) {
                        switch (MultiRowStrategy) {
                            case MultiRowStrategy.Stack:
                                foreach (var item in e.OldItems) {
                                    View childToRemove = null;

                                    foreach (Layout<View> row in Children) {
                                        childToRemove = row.Children.FirstOrDefault(v => v.BindingContext == item);

                                        if (childToRemove != null) {
                                            break;
                                        }
                                    }

                                    Layout<View> rowStack = childToRemove.Parent as Layout<View>;
                                    rowStack.Children.Remove(childToRemove);

                                    if (rowStack.Children.Count == 0) {
                                        Children.Remove(rowStack);
                                    }
                                }
                                break;
                            case MultiRowStrategy.Grid:
                                foreach (var item in e.OldItems) {
                                    View childToRemove = null;

                                    Grid rowStack = Children.FirstOrDefault() as Grid;

                                    childToRemove = rowStack.Children.FirstOrDefault(v => v.BindingContext == item);
                                    if (childToRemove == null) {
                                        break;
                                    }

                                    rowStack.Children.Remove(childToRemove);

                                    int row = -1;

                                    for (int i = 0; i < rowStack.Children.Count; i++) {
                                        View child = rowStack.Children[i];

                                        row = i / ItemsPerRow;

                                        Grid.SetColumn(child, i % ItemsPerRow);
                                        if (StretchLastOddElement) {
                                            if (i == ItemsSource.Count - 1 && (i % ItemsPerRow) == 0) {
                                                Grid.SetColumnSpan(child, ItemsPerRow);
                                            }
                                        }
                                        Grid.SetRow(child, row);
                                    }

                                    List<RowDefinition> rowsToRemove = rowStack.RowDefinitions.SkipWhile((r, i) => i <= row).ToList();

                                    rowsToRemove.ForEach(r => rowStack.RowDefinitions.Remove(r));
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else {
                        foreach (object item in e.OldItems) {
                            View childToRemove = Children.FirstOrDefault(v => v.BindingContext == item);
                            Children.Remove(childToRemove);
                        }
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset) {
                    ClearChildren();
                }
                else {
                    //
                    // TODO: handle other actions...
                    //
                    throw new NotImplementedException("StackList.StackList_CollectionChanged");
                }
            }
            catch (Exception exc) {
                Crashes.TrackError(exc, new Dictionary<string, string>() { { "Method", "StackList.StackList_CollectionChanged" } });
                throw;
            }
        }

        private void FillLayout() {
            ClearChildren();

            if (ItemsSource == null) {
                return;
            }

            try {
                if (IsMultiRowEnabled) {

                    Layout<View> rowStack = null;

                    switch (MultiRowStrategy) {
                        case MultiRowStrategy.Stack:
                            rowStack = BuildStackRow();

                            foreach (object item in ItemsSource) {
                                SourceItemBase singleItem = PrepareSingleItem(item);
                                rowStack.Children.Add(singleItem);

                                if (rowStack.Children.Count == ItemsPerRow) {
                                    Children.Add(rowStack);
                                    rowStack = BuildStackRow();
                                }
                            }
                            break;
                        case MultiRowStrategy.Grid:
                            rowStack = BuildGridRow(ItemsPerRow);

                            for (int i = 0; i < ItemsSource.Count; i++) {
                                SourceItemBase singleItem = PrepareSingleItem(ItemsSource[i]);
                                rowStack.Children.Add(singleItem);

                                int row = i / ItemsPerRow;

                                if (((Grid)rowStack).RowDefinitions.Count < (row + 1)) {
                                    RowDefinition rowDefinition = new RowDefinition();
                                    rowDefinition.Height = GridLength.Auto;

                                    ((Grid)rowStack).RowDefinitions.Add(rowDefinition);
                                }

                                Grid.SetColumn(singleItem, i % ItemsPerRow);
                                if (StretchLastOddElement) {
                                    if (i == ItemsSource.Count - 1 && (i % ItemsPerRow) == 0) {
                                        Grid.SetColumnSpan(singleItem, ItemsPerRow);
                                    }
                                }
                                Grid.SetRow(singleItem, row);
                            }
                            break;
                        default:
                            break;
                    }

                    if (rowStack.Children.Any()) {
                        Children.Add(rowStack);
                    }
                }
                else {
                    //
                    // TODO: refactor layout filling and items creation
                    //
                    if (IsGrouped) {
                        foreach (object group in ItemsSource) {
                            Children.Add(PrepareGroup(group));
                        }
                    }
                    else {
                        foreach (object item in ItemsSource) {
                            Children.Add(PrepareSingleItem(item));
                        }
                    }
                }
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);
                throw;
            }
        }

        private Grid BuildGridRow(int columnsCount) {
            Grid stackRow = new Grid();
            stackRow.HorizontalOptions = RowHorizontalLayoutOptions;
            stackRow.ColumnSpacing = Spacing;
            stackRow.RowSpacing = Spacing;

            for (int i = 0; i < columnsCount; i++) {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(1, GridUnitType.Star);
                stackRow.ColumnDefinitions.Add(columnDefinition);
            }

            return stackRow;
        }

        private StackLayout BuildStackRow() {
            StackLayout stackRow = new StackLayout();
            stackRow.Orientation = StackOrientation.Horizontal;
            stackRow.HorizontalOptions = RowHorizontalLayoutOptions;

            return stackRow;
        }

        private View PrepareGroup(object group) {
            try {
                StackLayout groupSpot = new StackLayout() {
                    Spacing = 0,
                    BindingContext = group
                };

                groupSpot.Children.Add((View)GroupHeaderTemplate.CreateContent());

                foreach (var item in (IList)group) {
                    groupSpot.Children.Add(PrepareSingleItem(item));
                }

                return groupSpot;
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                throw new InvalidOperationException(
                    string.Format("StackList.PrepareGroup - {0}. Exception details - {1}",
                    _ERORR_LAYOUT_FILLING,
                    exc.Message), exc);
            }
        }

        private void ClearChildren() {
            _stackListSingleItemsChildren.Clear();
            Children?.Clear();
        }

        private void OnItemSelected(SourceItemBase stackListItem) {
            if (_selectedStackListItem != null) {
                _selectedStackListItem.Deselected();
            }

            _selectedStackListItem = stackListItem;
            _selectedStackListItem.Selected();

            SelectedItem = stackListItem.BindingContext;
        }

        private SourceItemBase PrepareSingleItem(object item) {
            try {
                SourceItemBase stackListItem = (ItemTemplate is DataTemplate)
                    ? (ItemTemplate is DataTemplateSelector)
                        ? (SourceItemBase)((DataTemplateSelector)ItemTemplate).SelectTemplate(item, this).CreateContent()
                        : (SourceItemBase)ItemTemplate.CreateContent()
                    : throw new Exception("Can't resolve ItemTemplate type.");

                stackListItem.SelectionAction = OnItemSelected;
                stackListItem.BindingContext = item;

                _stackListSingleItemsChildren.Add(stackListItem);

                return stackListItem;
            }
            catch (Exception exc) {
                Crashes.TrackError(exc);

                throw new InvalidOperationException(
                    string.Format("StackList.PrepareSingleItem - {0}. Exception details - {1}",
                    _ERORR_LAYOUT_FILLING,
                    exc.Message), exc);
            }
        }
    }
}
