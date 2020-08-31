using Android.Content;
using Android.Views;
using PeakMVP.Controls;
using PeakMVP.Droid.Renderers;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(EntryEx), typeof(EntryExRenderer))]
namespace PeakMVP.Droid.Renderers {
    public class EntryExRenderer : EntryRenderer {

        private BorderRenderer _renderer;

        private readonly Context _context;

        private const GravityFlags DefaultGravity = GravityFlags.CenterVertical;

        /// <summary>
        ///     ctor().
        /// </summary>
        /// <param name="context"></param>
        public EntryExRenderer(Context context):base(context) {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e) {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
                return;
            Control.Gravity = DefaultGravity;
            var entryEx = Element as EntryEx;
            UpdateBackground(entryEx);
            UpdatePadding(entryEx);
            UpdateTextAlighnment(entryEx);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(sender, e);
            if (Element == null)
                return;
            var entryEx = Element as EntryEx;
            if (e.PropertyName == EntryEx.BorderWidthProperty.PropertyName ||
                e.PropertyName == EntryEx.BorderColorProperty.PropertyName ||
                e.PropertyName == EntryEx.BorderRadiusProperty.PropertyName ||
                e.PropertyName == EntryEx.BackgroundColorProperty.PropertyName) {
                UpdateBackground(entryEx);
            } else if (e.PropertyName == EntryEx.LeftPaddingProperty.PropertyName ||
                  e.PropertyName == EntryEx.RightPaddingProperty.PropertyName) {
                UpdatePadding(entryEx);
            } else if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName) {
                UpdateTextAlighnment(entryEx);
            }
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing) {
                if (_renderer != null) {
                    _renderer.Dispose();
                    _renderer = null;
                }
            }
        }

        private void UpdateBackground(EntryEx entryEx) {
            if (_renderer != null) {
                _renderer.Dispose();
                _renderer = null;
            }
            _renderer = new BorderRenderer();

            Control.Background = _renderer.GetBorderBackground(entryEx.BorderColor, entryEx.BackgroundColor, entryEx.BorderWidth, entryEx.BorderRadius);
        }

        private void UpdatePadding(EntryEx entryEx) {
            Control.SetPadding((int)_context.ToPixels(entryEx.LeftPadding), 0,
                (int)_context.ToPixels(entryEx.RightPadding), 0);
        }

        private void UpdateTextAlighnment(EntryEx entryEx) {
            var gravity = DefaultGravity;
            switch (entryEx.HorizontalTextAlignment) {
                case Xamarin.Forms.TextAlignment.Start:
                    gravity |= GravityFlags.Start;
                    break;
                case Xamarin.Forms.TextAlignment.Center:
                    gravity |= GravityFlags.CenterHorizontal;
                    break;
                case Xamarin.Forms.TextAlignment.End:
                    gravity |= GravityFlags.End;
                    break;
            }
            Control.Gravity = gravity;
        }
    }
}