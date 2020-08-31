using PeakMVP.Models.DataItems.Autorization;
using PeakMVP.Models.Rests.Requests.RequestDataModels;
using PeakMVP.Validations.ValidationRules;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PeakMVP.ViewModels.Authorization.Registration {
    public class ParentRegistrationInputFormViewModel : CommonRegistrationInputFormViewModel {

        private static readonly string _PARENT_LOGIN_DETAILS_PREFIX = "Parent";

        public ParentRegistrationInputFormViewModel() {
            OnPropertyChanged("Children.Count");

            LoginDetailsTitle = string.Format("{0} {1}", _PARENT_LOGIN_DETAILS_PREFIX, _LOGIN_DETAILS_COMMON_TITLE);
        }

        public ICommand AddChildCommand => new Command(() => {
            Children.Add(BuildSingleChildViewModelItem());

            OnPropertyChanged("Children.Count");
        });

        public ICommand RemoveChildCommand => new Command<ChildRegistrationInputFormViewModel>((ChildRegistrationInputFormViewModel childViewModel) => {
            Children.Remove(childViewModel);

            OnPropertyChanged("Children.Count");
        });

        ObservableCollection<ChildRegistrationInputFormViewModel> _children = new ObservableCollection<ChildRegistrationInputFormViewModel>();
        public ObservableCollection<ChildRegistrationInputFormViewModel> Children {
            get => _children;
            set => SetProperty<ObservableCollection<ChildRegistrationInputFormViewModel>>(ref _children, value);
        }

        public override void Dispose() {
            base.Dispose();

            Children.ForEach(cVM => cVM.Dispose());
        }

        public override bool ValidateForm() {
            bool isValidResult = base.ValidateForm();

            Children.ForEach<ChildRegistrationInputFormViewModel>(cVM => cVM.ValidateForm());
            bool childrenFormValidationResult = !Children.Any<ChildRegistrationInputFormViewModel>(cVM => cVM.IsFormValid == false);

            isValidResult = isValidResult && childrenFormValidationResult;

            return isValidResult;
        }

        public override void ResetInputForm() {
            base.ResetInputForm();

            Children.Clear();
            OnPropertyChanged("Children.Count");
        }

        public override RegistrationRequestDataModel BuildRegistrationDataModel() {
            RegistrationRequestDataModel registrationRequestDataModel = base.BuildRegistrationDataModel();
            registrationRequestDataModel.Type = ProfileType.Parent.ToString();
            registrationRequestDataModel.Children = Children.Select(cVM => cVM.BuildRegistrationDataModel()).ToArray();

            return registrationRequestDataModel;
        }

        protected override void ResetValidationObjects() {
            base.ResetValidationObjects();

            DateOfBirth.Value = new DateTime((DateTime.Now - TimeSpan.FromDays(_DAYS_PER_YEAR * _ADULT_AGE_RESTRICTION)).Ticks);
            DateOfBirth.Validations.Add(new DateRule<DateTime> { ValidationMessage = _TO_YOUNG_ERROR_MESSAGE, DaysRestriction = TimeSpan.FromDays(_DAYS_PER_YEAR * _ADULT_AGE_RESTRICTION) });
        }

        private ChildRegistrationInputFormViewModel BuildSingleChildViewModelItem() {
            ChildRegistrationInputFormViewModel childViewModel = new ChildRegistrationInputFormViewModel();

            return childViewModel;
        }
    }
}
