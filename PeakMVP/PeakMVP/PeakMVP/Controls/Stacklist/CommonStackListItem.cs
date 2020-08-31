using PeakMVP.Controls.Stacklist.Base;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PeakMVP.Controls.Stacklist {
    public class CommonStackListItem : SourceItemBase {

        private static readonly Color DEFAULT_SELECTED_COLOR = Color.LightGray;
        private static readonly Color DEFAULT_DESELECTED_COLOR = Color.Transparent;

        public CommonStackListItem() {
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();

            tapGestureRecognizer.Tapped += OnTapGestureRecognizerTapped;

            GestureRecognizers.Add(tapGestureRecognizer);
        }

        public bool IsSelectable { get; set; } = false;

        public bool IsOnSelectionVisualChangesEnabled { get; set; } = false;

        public Color SelectedColor { get; set; } = DEFAULT_SELECTED_COLOR;

        public Color DeselectedColor { get; set; } = DEFAULT_DESELECTED_COLOR;

        public override void Deselected() {
            if (IsOnSelectionVisualChangesEnabled) {
                BackgroundColor = DeselectedColor;
            }
        }

        public override void Selected() {
            if (IsOnSelectionVisualChangesEnabled) {
                BackgroundColor = SelectedColor;
            }
        }

        private void OnTapGestureRecognizerTapped(object sender, EventArgs e) {
            if (IsSelectable && SelectionAction != null) {
                SelectionAction(this);
            }
        }
    }
}
