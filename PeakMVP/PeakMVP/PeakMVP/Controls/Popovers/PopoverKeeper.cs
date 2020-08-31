using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PeakMVP.Controls.Popovers.Arguments;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.Controls.Popovers {
    public class PopoverKeeper : AbsoluteLayout, IPopoverKeeper {

        private static readonly double _BOTTOM_BASED_POPOVER_Y_OFFSET = 4;

        private TapGestureRecognizer _backingTapGesture = new TapGestureRecognizer();

        private BoxView _backing;

        public PopoverKeeper() {
            Init();
        }

        private void Init() {
            _backingTapGesture.Tapped += PopoverBackingTapped;

            _backing = new BoxView();
            _backing.Opacity = .9;
            _backing.BackgroundColor = Color.Transparent;

            _backing.GestureRecognizers.Add(_backingTapGesture);

            AbsoluteLayout.SetLayoutFlags(_backing, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(_backing, new Rectangle(1, 1, 1, 1));
        }

        public void HideAllPopovers() {
            Children.Remove(_backing);

            IPopover[] popoversToRemove =
                Children
                .OfType<IPopover>()
                .ToArray<IPopover>();

            popoversToRemove.ForEach<IPopover>(p => {
                p.IsPopoverVisible = false;
                Children.Remove((View)p);
            });
        }

        public void HidePopover(IPopover popover) {
            if (popover != null && popover is View) {
                popover.IsPopoverVisible = false;
                Children.Remove(_backing);
                Children.Remove((View)popover);
            }
        }

        public void ShowPopover(IPopover popover, ShowPopoverArgs showPopoverArgs) {
            if (popover != null && popover is View) {
                Children.Add(_backing);
                Children.Add((View)popover);

                double xTransition = default(double);
                double yTransition = default(double);

                switch (showPopoverArgs.PopoverLayoutingStrategy) {
                    case PopoverLayoutingStrategy.LeftOverlay:
                        xTransition = showPopoverArgs.DropDownSelectrorRectangle.Left;
                        yTransition = showPopoverArgs.DropDownSelectrorRectangle.Top;
                        break;
                    case PopoverLayoutingStrategy.LeftBottom:
                        xTransition = showPopoverArgs.DropDownSelectrorRectangle.Left;
                        yTransition = showPopoverArgs.DropDownSelectrorRectangle.Top + showPopoverArgs.DropDownSelectrorRectangle.Height + PopoverKeeper._BOTTOM_BASED_POPOVER_Y_OFFSET;
                        break;
                    case PopoverLayoutingStrategy.RightOverlay:
                        xTransition = showPopoverArgs.DropDownSelectrorRectangle.Left + (showPopoverArgs.DropDownSelectrorRectangle.Width - ((View)popover).Width);
                        yTransition = showPopoverArgs.DropDownSelectrorRectangle.Top;
                        break;
                    case PopoverLayoutingStrategy.RightBottom:
                        xTransition = showPopoverArgs.DropDownSelectrorRectangle.Left + (showPopoverArgs.DropDownSelectrorRectangle.Width - ((View)popover).Width);
                        yTransition = showPopoverArgs.DropDownSelectrorRectangle.Top + showPopoverArgs.DropDownSelectrorRectangle.Height + PopoverKeeper._BOTTOM_BASED_POPOVER_Y_OFFSET;
                        break;
                    default:
                        throw new InvalidOperationException("PopoverKeeperControl ShowPopover invalid PopoverLayoutingStrategy");
                }

                ((View)popover).TranslationX = xTransition;
                ((View)popover).TranslationY = yTransition;

                if (popover.IsHaveSameWidth) {
                    ((View)popover).WidthRequest = showPopoverArgs.DropDownSelectrorRectangle.Width;
                }

                popover.IsPopoverVisible = true;
            }
        }

        private void PopoverBackingTapped(object sender, EventArgs e) => HideAllPopovers();
    }
}