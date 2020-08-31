using Xamarin.Forms;

namespace PeakMVP.Controls.ActionBars.Base {
    public abstract class ActionBarBase : ContentView {

        private static readonly string _BINDING_CONTEXT_SOURCE_PATH = "ActionBarViewModel";

        public ActionBarBase() {

            SetBinding(BindingContextProperty, new Binding(_BINDING_CONTEXT_SOURCE_PATH));
        }
    }
}