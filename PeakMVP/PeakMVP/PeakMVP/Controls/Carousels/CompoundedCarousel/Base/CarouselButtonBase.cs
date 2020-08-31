using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace PeakMVP.Controls.Carousels.CompoundedCarousel.Base {
    public abstract class CarouselButtonBase : ContentView {

        public static readonly BindableProperty IconProperty =
            BindableProperty.Create("Icon",
                typeof(ImageSource),
                typeof(CarouselButtonBase),
                defaultValue: default(ImageSource),
                propertyChanged: OnIconPropertyChanged);

        public CarouselButtonBase() { }

        public ImageSource Icon {
            get => (ImageSource)GetValue(CarouselButtonBase.IconProperty);
            set => SetValue(CarouselButtonBase.IconProperty, value);
        }

        private static void OnIconPropertyChanged(BindableObject bindable, object oldValue, object newValue) {
            CarouselButtonBase carouselButtonBase = bindable as CarouselButtonBase;

            if (carouselButtonBase != null) {
                carouselButtonBase.OnIconChanged();
            }
        }

        protected virtual void OnIconChanged() { }
    }
}