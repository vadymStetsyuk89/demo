using Foundation;
using PeakMVP.Controls;
using PeakMVP.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ListViewExtended), typeof(ListViewExtendedRenderer))]
namespace PeakMVP.iOS.Renderers {

    public class TODO : UIGestureRecognizer {
        public override void PressesEnded(NSSet<UIPress> presses, UIPressesEvent evt) {
            base.PressesEnded(presses, evt);
        }

        public override void PressesCancelled(NSSet<UIPress> presses, UIPressesEvent evt) {
            base.PressesCancelled(presses, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt) {
            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt) {
            base.TouchesCancelled(touches, evt);
        }
    }

    public class ListViewExtendedRenderer : ListViewRenderer {

        public ListViewExtendedRenderer() {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e) {
            base.OnElementChanged(e);

            TODO tODO = new TODO();
            Control.AddGestureRecognizer(tODO);
        }
    }
}