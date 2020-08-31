using PeakMVP.Controls.Carousels.CompoundedCarousel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Controls.Carousels.CompoundedCarousel.Buttons {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SquareButton : CarouselButtonBase {

        public SquareButton() {
            InitializeComponent();
        }

        protected override void OnIconChanged() {
            base.OnIconChanged();

            _buttonImage_CachedImage.Source = Icon;
        }
    }
}