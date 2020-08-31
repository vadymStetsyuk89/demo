using PeakMVP.Controls.Popovers.Base;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Controls.Popovers {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListedPopover : PopoverBase {

        public static readonly BindableProperty ListPaddingProperty = BindableProperty.Create(
            nameof(ListPadding),
            typeof(Thickness),
            typeof(ListedPopover),
            defaultValue: default(Thickness));

        public static readonly BindableProperty ListSpacingProperty = BindableProperty.Create(
            nameof(ListSpacing),
            typeof(double),
            typeof(ListedPopover),
            defaultValue: 6.0);

        public ListedPopover() {
            InitializeComponent();

            _itemsList_StackList.SetBinding(Xamarin.Forms.Layout.PaddingProperty, new Binding(nameof(ListPadding), source: this));
            _itemsList_StackList.SetBinding(StackLayout.SpacingProperty, new Binding(nameof(ListSpacing), source: this));
        }

        public Thickness ListPadding {
            get => (Thickness)GetValue(ListPaddingProperty);
            set => SetValue(ListPaddingProperty, value);
        }

        public double ListSpacing {
            get => (double)GetValue(ListSpacingProperty);
            set => SetValue(ListSpacingProperty, value);
        }

        private DataTemplate _itemTemplate;
        public DataTemplate ItemTemplate {
            get => _itemTemplate;
            set {
                _itemTemplate = value;

                try {
                    _itemsList_StackList.ItemTemplate = ItemTemplate;
                }
                catch (Exception exc) {
                    Debugger.Break();
                    throw new InvalidOperationException("ListedPopover ItemTemplate setter", exc);
                }
            }
        }
    }
}