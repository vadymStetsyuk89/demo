using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace PeakMVP.Controls.Stacklist.Base {
    public abstract class SourceItemBase : ContentView {
        public SourceItemBase() {
            ItemSelectCommand = new Command(() => {
                SelectionAction(this);
            });
        }

        public Command ItemSelectCommand { get; private set; }

        public Action<SourceItemBase> SelectionAction { get; set; }

        public abstract void Selected();

        public abstract void Deselected();
    }
}