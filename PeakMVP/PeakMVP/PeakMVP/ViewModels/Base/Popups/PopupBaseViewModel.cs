using PeakMVP.Controls.Popups;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PeakMVP.ViewModels.Base.Popups {
    public abstract class PopupBaseViewModel : NestedViewModel, IPopupContext {

        public ICommand ShowPopupCommand => new Command((object param) => {
            OnShowPopupCommand(param);
            UpdatePopupScopeVisibility(true);
            IsPopupVisible = true;
        });

        public ICommand ClosePopupCommand => new Command((object param) => {
            OnClosePopupCommand(param);
            UpdatePopupScopeVisibility(false);
        });

        private bool _isPopupVisible;
        public bool IsPopupVisible {
            get => _isPopupVisible;
            set {
                SetProperty<bool>(ref _isPopupVisible, value);

                OnIsPopupVisible();
            }
        }

        private string _title;
        public string Title {
            get => _title;
            protected set => SetProperty<string>(ref _title, value);
        }

        public abstract Type RelativeViewType { get; }

        public override Task InitializeAsync(object navigationData) {
            if (navigationData is ContentPageBaseViewModel pageBaseViewModel) {
                if (!pageBaseViewModel.Popups.Contains(this)) {
                    pageBaseViewModel.Popups.Add(this);
                }
            }

            return base.InitializeAsync(navigationData);
        }

        protected virtual void OnIsPopupVisible() { }

        protected virtual void OnShowPopupCommand(object param) { }

        protected virtual void OnClosePopupCommand(object param) { }
    }
}
