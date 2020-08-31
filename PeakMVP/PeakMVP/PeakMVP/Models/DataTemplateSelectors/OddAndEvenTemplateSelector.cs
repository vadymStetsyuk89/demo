using PeakMVP.Controls.Stacklist;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace PeakMVP.Models.DataTemplateSelectors {
    public class OddAndEvenTemplateSelector : DataTemplateSelector {

        public DataTemplate OddItemTemplate { get; set; }

        public DataTemplate EvenItemTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container) {

            IEnumerable<object> items = null;

            if (container is ListView) {
                items = ((ListView)container).ItemsSource.Cast<object>();
            }
            else if (container is StackList) {
                items = ((StackList)container).ItemsSource.Cast<object>();
            }
            else {
                throw new InvalidOperationException("OddAndEvenTemplateSelector.OnSelectTemplate cant resolve container type.");
            }

            bool isEven = (item != null) ? items.ToList().IndexOf(item) % 2 == 0 : false;

            return (isEven) ? EvenItemTemplate : OddItemTemplate;
        }
    }
}
