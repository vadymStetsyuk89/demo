using PeakMVP.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeakMVP.Services.Navigation {
    public interface INavigationService {

        bool CanVisibleBackButton { get; }

        ViewModelBase PreviousPageViewModel { get; }

        ViewModelBase LastPageViewModel { get; }

        IReadOnlyCollection<ViewModelBase> CurrentViewModelsNavigationStack { get; }

        void Initialize();

        void DisposeStack();

        void Initialize(bool tryToUpdateLocalUserProfile);

        Task NavigateToAsync(Type navigateTo);

        Task NavigateToAsync(Type navigateTo, object parameter);

        Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase;

        Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase;

        Task RemoveLastFromBackStackAsync();

        Task RemoveBackStackAsync();

        Task GoBackAsync();

        Task GoBackAsync(object arguments);
    }
}