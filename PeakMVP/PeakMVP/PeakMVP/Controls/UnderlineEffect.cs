using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PeakMVP.Controls
{
    public class UnderlineEffect : RoutingEffect {
        public const string EffectNamespace = "PeakMVP.Controls.Example";

        public UnderlineEffect() : base($"{EffectNamespace}.{nameof(UnderlineEffect)}") {
        }
    }
}
