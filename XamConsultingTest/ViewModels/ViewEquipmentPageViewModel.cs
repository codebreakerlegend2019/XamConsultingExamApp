using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using XamConsultingTest.Interfaces;
using XamConsultingTest.Models;

namespace XamConsultingTest.ViewModels
{
    public class ViewEquipmentPageViewModel : ViewModelBase, INavigatedAware
    {
        private readonly INavigationService _navigationService;
        private readonly IGet<Equipment> _equipmentGetService;

        public Equipment Equipment { get; set; }

        public ViewEquipmentPageViewModel(INavigationService navigationService, IGet<Equipment> equipmentGetService) : base(navigationService)
        {
            this._navigationService = navigationService;
            this._equipmentGetService = equipmentGetService;

        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.Count > 0)
            {
                var equipment = _equipmentGetService.Get(parameters.GetValue<int>("id"));
                if (equipment == null)
                {
                    await App.Current.MainPage.DisplayAlert("Info", "Equipment not found", "Ok");
                    await _navigationService.GoBackAsync();
                }
                else
                {
                    Title = equipment.Name;
                    Equipment = equipment;
                    RaisePropertyChanged(nameof(Equipment));
                }
            }
        }

        public DelegateCommand GoBackCommand => new DelegateCommand(async()=> 
        {
            await _navigationService.GoBackAsync();
        });
    }
}
