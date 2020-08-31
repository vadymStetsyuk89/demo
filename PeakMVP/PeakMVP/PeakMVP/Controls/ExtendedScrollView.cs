using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.Controls
{
    public class ExtendedScrollView : ScrollView
    {

        public static readonly BindableProperty RequestToScrollToEndCommandProperty =
            BindableProperty.Create(
                nameof(RequestToScrollToEndCommand),
                typeof(ICommand),
                typeof(ExtendedScrollView),
                defaultBindingMode: BindingMode.OneWayToSource);

        public ExtendedScrollView()
        {
            RequestToScrollToEndCommand = new Command(() =>
            {
                try
                {

                    if (Content != null)
                    {
                        if (Device.RuntimePlatform == Device.iOS)
                        {
                            double targetY = Content.Height - Height;
                            ScrollToAsync(0, targetY > 0 ? targetY : 0, true);
                        }
                        else
                        {
                            ScrollToAsync(0, Content.Height, true);
                        }
                    }
                }
                catch (System.Exception exc)
                {
                    Debugger.Break();
                }
            });
        }

        public bool AutoScrollDownWhenContentChanged { get; set; }

        public ICommand RequestToScrollToEndCommand
        {
            get { return (ICommand)GetValue(RequestToScrollToEndCommandProperty); }
            set { SetValue(RequestToScrollToEndCommandProperty, value); }
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);

            if (AutoScrollDownWhenContentChanged)
            {
                Element element = this.Content;

                if (element != null)
                {
                    ScrollToAsync(element, ScrollToPosition.End, false);
                }
            }
        }
    }
}