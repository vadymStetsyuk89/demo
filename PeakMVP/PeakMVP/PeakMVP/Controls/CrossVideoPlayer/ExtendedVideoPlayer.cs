using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace PeakMVP.Controls.CrossVideoPlayer {
    public class ExtendedVideoPlayer : ContentView {

        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(propertyName: "Source",
                returnType: typeof(string),
                declaringType: typeof(ExtendedVideoPlayer),
                defaultValue: string.Empty);

        public static readonly BindableProperty IsAppearedProperty =
            BindableProperty.Create(propertyName: "IsAppeared",
                returnType: typeof(bool),
                declaringType: typeof(ExtendedVideoPlayer),
                defaultValue: false);

        public ExtendedVideoPlayer() { }

        /// <summary>
        /// TODO: try to define better solution
        /// Each platform renderer will listen to changes of this property and respond on invokation
        /// </summary>
        public Action ReleaseAction { get; set; }

        /// <summary>
        /// TODO: try to define better solution
        /// Each platform renderer will listen to changes of this property and respond on changes
        /// </summary>
        public bool IsAppeared {
            get { return (bool)GetValue(IsAppearedProperty); }
            set { SetValue(IsAppearedProperty, value); }
        }

        public string Source {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
    }
}