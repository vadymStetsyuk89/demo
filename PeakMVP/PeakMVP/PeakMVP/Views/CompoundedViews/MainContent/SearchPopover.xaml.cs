﻿using PeakMVP.Controls.Popovers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PeakMVP.Views.CompoundedViews.MainContent {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPopover : PopoverBase {
        public SearchPopover() {
            InitializeComponent();
        }
    }
}