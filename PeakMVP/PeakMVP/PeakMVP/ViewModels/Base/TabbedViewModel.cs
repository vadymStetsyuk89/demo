using System;

namespace PeakMVP.ViewModels.Base {
    public abstract class TabbedViewModel : NestedViewModel, IBottomBarTab {

        public TabbedViewModel() {
            TabbViewModelInit();
        }

        private bool _isBudgeVisible;
        public bool IsBudgeVisible {
            get => _isBudgeVisible;
            protected set => SetProperty<bool>(ref _isBudgeVisible, value);
        }

        private int _budgeCount;
        public int BudgeCount { get => _budgeCount; protected set => SetProperty<int>(ref _budgeCount, value); }

        public string TabHeader { get; protected set; }

        public string TabIcon { get; protected set; }

        public Type RelativeViewType { get; protected set; }

        protected abstract void TabbViewModelInit();
    }
}
