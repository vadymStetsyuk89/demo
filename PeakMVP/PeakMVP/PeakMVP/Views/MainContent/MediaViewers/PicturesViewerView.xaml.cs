﻿using PeakMVP.Views.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.MainContent.MediaViewers {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PicturesViewerView : ContentPageBase {

        public PicturesViewerView() {
            InitializeComponent();
        }
    }
}