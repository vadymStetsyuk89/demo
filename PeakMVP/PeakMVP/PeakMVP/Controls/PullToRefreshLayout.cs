using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.Controls {
    public class PullToRefreshLayout : ContentView {

        public static readonly BindableProperty IsRefreshingProperty =
            BindableProperty.Create(
                nameof(IsRefreshing),
                typeof(bool),
                typeof(PullToRefreshLayout),
                false);

        public static readonly BindableProperty IsPullToRefreshEnabledProperty =
            BindableProperty.Create(
                nameof(IsPullToRefreshEnabled),
                typeof(bool),
                typeof(PullToRefreshLayout),
                false);

        public static readonly BindableProperty RefreshCommandProperty =
            BindableProperty.Create(
                nameof(RefreshCommand),
                typeof(ICommand),
                typeof(PullToRefreshLayout));

        public static readonly BindableProperty RefreshCommandParameterProperty =
           BindableProperty.Create(
               nameof(RefreshCommandParameter),
               typeof(object),
               typeof(PullToRefreshLayout),
               null,
               propertyChanged: (bindable, oldvalue, newvalue) => ((PullToRefreshLayout)bindable).RefreshCommandCanExecuteChanged(bindable, EventArgs.Empty));

        public static readonly BindableProperty RefreshColorProperty =
            BindableProperty.Create(
                nameof(RefreshColor), 
                typeof(Color), 
                typeof(PullToRefreshLayout), 
                Color.Default);

        public static readonly BindableProperty RefreshBackgroundColorProperty =
            BindableProperty.Create(
                nameof(RefreshBackgroundColor), 
                typeof(Color),
                typeof(PullToRefreshLayout), 
                Color.Default);

        public PullToRefreshLayout() { }

        public bool IsRefreshing {
            get { return (bool)GetValue(IsRefreshingProperty); }
            set {
                if ((bool)GetValue(IsRefreshingProperty) == value)
                    OnPropertyChanged(nameof(IsRefreshing));

                SetValue(IsRefreshingProperty, value);
            }
        }

        public bool IsPullToRefreshEnabled {
            get { return (bool)GetValue(IsPullToRefreshEnabledProperty); }
            set { SetValue(IsPullToRefreshEnabledProperty, value); }
        }

        public ICommand RefreshCommand {
            get { return (ICommand)GetValue(RefreshCommandProperty); }
            set { SetValue(RefreshCommandProperty, value); }
        }

        public object RefreshCommandParameter {
            get { return GetValue(RefreshCommandParameterProperty); }
            set { SetValue(RefreshCommandParameterProperty, value); }
        }

        public Color RefreshColor {
            get { return (Color)GetValue(RefreshColorProperty); }
            set { SetValue(RefreshColorProperty, value); }
        }

        public Color RefreshBackgroundColor {
            get { return (Color)GetValue(RefreshBackgroundColorProperty); }
            set { SetValue(RefreshBackgroundColorProperty, value); }
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint) {
            if (Content == null) {
                return new SizeRequest(new Size(100, 100));
            }

            return base.OnMeasure(widthConstraint, heightConstraint);
        }

        private void RefreshCommandCanExecuteChanged(object sender, EventArgs eventArgs) {
            ICommand cmd = RefreshCommand;
            if (cmd != null) {
                IsEnabled = cmd.CanExecute(RefreshCommandParameter);
            }
        }
    }
}