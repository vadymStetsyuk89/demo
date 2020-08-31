using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PeakMVP.Controls {
    public class ExtendedLabel : Label {

        public static readonly BindableProperty LineHeightProperty =
            BindableProperty.Create("LineHeight",
                typeof(float),
                typeof(ExtendedLabel),
                defaultValue: 1F);

        public float LineHeight {
            get => (float)GetValue(LineHeightProperty);
            set => SetValue(LineHeightProperty, value);
        }
    }
}
