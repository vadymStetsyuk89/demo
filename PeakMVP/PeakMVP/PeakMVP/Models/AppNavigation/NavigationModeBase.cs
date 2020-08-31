using PeakMVP.Helpers;
using PeakMVP.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Internals;

namespace PeakMVP.Models.AppNavigation {
    public abstract class NavigationModeBase : ObservableObject, IDisposable {

        public abstract string ModeIconPath { get; }

        public abstract BarMode BarModeType { get; }

        private int _selectedBarItemIndex;
        public int SelectedBarItemIndex {
            get => _selectedBarItemIndex;
            set => SetProperty<int>(ref _selectedBarItemIndex, value);
        }

        public IReadOnlyCollection<IBottomBarTab> BarItems { get; protected set; }

        public virtual void Dispose() { }

        public IBottomBarTab SelectTab(Type targetTabType) {
            IBottomBarTab resolvedTab = BarItems.FirstOrDefault(bottomTab => bottomTab.GetType().Equals(targetTabType));

            if (resolvedTab != null) {
                SelectedBarItemIndex = BarItems.IndexOf(resolvedTab);
            }

            return resolvedTab;
        }
    }
}
