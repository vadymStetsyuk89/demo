using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using Xamarin.Forms;

namespace PeakMVP.Behaviors {
    public class FFColorScaleBehavior : Behavior<CachedImage> {

        public Color TargetColor { get; set; }

        public float Brightness { get; set; }

        protected override void OnAttachedTo(CachedImage bindable) {
            base.OnAttachedTo(bindable);

            if (bindable != null) {
                bindable.Transformations.Add(new ColorSpaceTransformation(TargetColor.ColorToMatrix(Brightness)));
            }
        }
    }
}
