using FFImageLoading.Forms;
using Xamarin.Forms;

namespace PeakMVP.Controls {
    public class CachedImageExtended : CachedImage {

        public bool IsHeightRestrictionEnabled { get; set; } = true;

        public double HeightRestriction { get; set; } = 380;

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint) {
            SizeRequest sizeRequest = base.OnMeasure(widthConstraint, heightConstraint);

            if (IsHeightRestrictionEnabled) {
                System.Console.WriteLine("===> IsHeightRestrictionEnabled");
                return new SizeRequest(new Size(sizeRequest.Request.Width, sizeRequest.Request.Height > HeightRestriction ? HeightRestriction : sizeRequest.Request.Height), sizeRequest.Minimum);
            }

            return sizeRequest;
        }
    }
}
