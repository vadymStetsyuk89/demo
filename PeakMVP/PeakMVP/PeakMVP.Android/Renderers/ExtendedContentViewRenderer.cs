using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using PeakMVP.Controls;
using PeakMVP.Droid.Renderers;
using PeakMVP.Droid.Renderers.Helpers;
using PeakMVP.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.Views.View;

[assembly: ExportRenderer(typeof(ExtendedContentView), typeof(ExtendedContentViewRenderer))]
namespace PeakMVP.Droid.Renderers {
    public sealed class ExtendedContentViewRenderer : ViewRenderer {

        private static readonly PorterDuff.Mode _MASK_DSTIN_PORTER_DUFF = PorterDuff.Mode.DstIn;
        private static readonly float _DEFAULT_BORDER_WIDTH = 4;
        private static readonly Android.Graphics.Color _DEFAULT_BORDER_COLOR = Android.Graphics.Color.Red;

        private Paint _maskPaint;
        private Paint _borderPaint;

        private Path _maskPath;
        private Path _borderPath;

        /// <summary>
        /// Public ctor
        /// </summary>
        /// <param name="context"></param>
        public ExtendedContentViewRenderer(Context context) : base(context) {
            ///
            /// Allows to draw itself (in other case Draw(Canvas canvas) will not be occured).
            /// 
            SetWillNotDraw(false);

            ///
            /// Resolves 'black masked' area (for Xfermode consuming).
            /// 
            if (((int)Android.OS.Build.VERSION.SdkInt) >= 11) {
                SetLayerType(LayerType.Hardware, null);
            }

            Init();
        }

        private float _resolvedCornerRadius;
        public float ResolvedCornerRadius {
            get => _resolvedCornerRadius;
            private set {
                _resolvedCornerRadius = TypedValue.ApplyDimension(ComplexUnitType.Dip, value, Context.Resources.DisplayMetrics);
            }
        }

        private float _resolvedBorderThickness;
        public float ResolvedBorderThickness {
            get => _resolvedBorderThickness;
            private set {
                _resolvedBorderThickness = TypedValue.ApplyDimension(ComplexUnitType.Dip, value, Context.Resources.DisplayMetrics);

                _borderPaint.StrokeWidth = _resolvedBorderThickness;
                _maskPaint.StrokeWidth = _resolvedBorderThickness;
            }
        }

        private Android.Graphics.Color _resolvedBorderColor;
        public Android.Graphics.Color ResolvedBorderColor {
            get => _resolvedBorderColor;
            private set {
                _resolvedBorderColor = value;

                _borderPaint.Color = _resolvedBorderColor;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e) {
            base.OnElementChanged(e);

            if (Element != null) {
                ResolvedCornerRadius = ((ExtendedContentView)Element).CornerRadius;

                ResolvedBorderThickness = ((ExtendedContentView)Element).BorderThickness;
                ResolvedBorderColor = BaseSingleton<ValuesResolver>.Instance.ResolveNativeColor(((ExtendedContentView)Element).BorderColor);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ExtendedContentView.BorderColorProperty.PropertyName) {
                ResolvedBorderColor = BaseSingleton<ValuesResolver>.Instance.ResolveNativeColor(((ExtendedContentView)Element).BorderColor);

                /// 
                /// Invalidate() call, will force view, to redraw itself
                /// 
                Invalidate();
            }
            else if (e.PropertyName == ExtendedContentView.BorderThicknessProperty.PropertyName) {
                ResolvedBorderThickness = ((ExtendedContentView)Element).BorderThickness;

                /// 
                /// Invalidate() call, will force view, to redraw itself
                /// 
                Invalidate();
            }
            else if (e.PropertyName == ExtendedContentView.CornerRadiusProperty.PropertyName) {
                ResolvedCornerRadius = ((ExtendedContentView)Element).CornerRadius;

                /// 
                /// Invalidate() call, will force view, to redraw itself
                /// 
                Invalidate();
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            _maskPath = BuildRoundedPath(MeasuredWidth, MeasuredHeight, ResolvedCornerRadius, ResolvedBorderThickness);
            _borderPath = BuildRoundedPath(MeasuredWidth, MeasuredHeight, ResolvedCornerRadius, ResolvedBorderThickness);
        }

        public override void Draw(Canvas canvas) {
            try {
                canvas.Save();

                base.Draw(canvas);

                canvas.DrawPath(_maskPath, _maskPaint);
                canvas.DrawPath(_borderPath, _borderPaint);
                canvas.Restore();

            }
            catch (System.Exception exc) {
            }
        }

        private void Init() {
            _borderPaint = new Paint(PaintFlags.AntiAlias) {
                StrokeWidth = _DEFAULT_BORDER_WIDTH,
                Color = _DEFAULT_BORDER_COLOR,
            };
            _borderPaint.SetStyle(Paint.Style.Stroke);

            _maskPaint = new Paint(PaintFlags.AntiAlias);
            _maskPaint.SetStyle(Paint.Style.FillAndStroke);
            _maskPaint.SetXfermode(new PorterDuffXfermode(_MASK_DSTIN_PORTER_DUFF));
        }

        private Path BuildRoundedPath(float width, float height, float radius, float offset = 0) {
            Path path = new Path();

            try {
                path.AddRoundRect(offset, offset, width - offset, height - offset, radius, radius, Path.Direction.Cw);
            }
            catch (System.Exception) { }

            return path;
        }
    }
}