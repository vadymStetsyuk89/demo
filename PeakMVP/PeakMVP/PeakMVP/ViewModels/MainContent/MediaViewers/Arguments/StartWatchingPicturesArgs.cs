using PeakMVP.Models.Rests.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakMVP.ViewModels.MainContent.MediaViewers.Arguments {
    public class StartWatchingPicturesArgs {

        public ProfileMediaDTO TargetMedia { get; set; }

        public IEnumerable<ProfileMediaDTO> MediasSource { get; set; }
    }
}
