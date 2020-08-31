using System;
using Xamarin.Forms;

namespace PeakMVP.Behaviors.Base {
    public class BindableBehavior<T> : Behavior<T> where T : BindableObject {

        public T AssociatedObject { get; private set; }

        protected override void OnAttachedTo(T visualElememt) {
            base.OnAttachedTo(visualElememt);

            AssociatedObject = visualElememt;

            if (visualElememt.BindingContext != null) {
                BindingContext = visualElememt.BindingContext;
            }

            visualElememt.BindingContextChanged += OnBindingContextChanged;
        }

        private void OnBindingContextChanged(object sender, EventArgs e) {
            OnBindingContextChanged();
        }

        protected override void OnDetachingFrom(T view) {
            view.BindingContextChanged -= OnBindingContextChanged;
        }

        protected override void OnBindingContextChanged() {
            base.OnBindingContextChanged();
            BindingContext = AssociatedObject.BindingContext;
        }
    }
}
