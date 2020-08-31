using PeakMVP.Models.Identities;
using PeakMVP.Views.CompoundedViews.MainContent.Messenger;
using Xamarin.Forms;

namespace PeakMVP.Models.DataTemplateSelectors {
    public class MessageTemplateSelector : DataTemplateSelector {

        private readonly DataTemplate _incomingDataTemplate;
        private readonly DataTemplate _outgoingDataTemplate;

        public MessageTemplateSelector() {
            _incomingDataTemplate = new DataTemplate(typeof(IncomingMessageStackItem));
            _outgoingDataTemplate = new DataTemplate(typeof(OutcomingMessageStackItem));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container) {
            MessageItem messageItem = item as MessageItem;

            if (messageItem == null) return null;

            return messageItem.IsIncomming ? _incomingDataTemplate : _outgoingDataTemplate;
        }
    }
}
