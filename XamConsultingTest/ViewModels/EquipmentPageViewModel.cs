using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using XamConsultingTest.Interfaces;
using XamConsultingTest.Models;
using XamConsultingTest.Views;

namespace XamConsultingTest.ViewModels
{
    public class EquipmentPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IGetAll<Equipment> _equipmentService;

        private Equipment _selectedEquipment;
        public Equipment SelectedEquipment
        {
            get=> _selectedEquipment;
            set
            {
                _selectedEquipment = value;
            }
        }
        public bool IsRefreshing { get; set; }
        public List<Equipment> Equipments { get; set; }

        private List<Equipment> _equipments { get; set; }

        private string _keyWord;
        public string KeyWord
        {
            get => _keyWord ?? "";
            set
            {
                _keyWord = value;
                filterEquipments(value.ToLower());
                RaisePropertyChanged();
            }
        }
        public EquipmentPageViewModel(INavigationService navigationService, IGetAll<Equipment> equipmentService)
            :base(navigationService)
        {
                this._navigationService = navigationService;
                this._equipmentService = equipmentService;
                Title = "Equipments";
        }

        private void loadItems()
        {
            var cacheEquipments = _equipmentService.GetAll();
            _equipments = cacheEquipments;
            Equipments = _equipments;
            RaisePropertyChanged(nameof(Equipments));
        }

        private void filterEquipments(string searchData)
        {
            if(_equipments != null)
            {
                var equipments = new List<Equipment>();
                foreach (var equipment in _equipments)
                {
                    var searchMatch = equipment.Name.ToLower().Contains(searchData)
                       || equipment.Comments.ToLower().Contains(searchData);
                    if (searchMatch)
                        equipments.Add(equipment);
                }
                Equipments = equipments;
                RaisePropertyChanged(nameof(Equipments));
            }
        }

        public DelegateCommand GoToAddEquipmentPageCommand => new DelegateCommand(async () =>
         {
             await _navigationService.NavigateAsync("AddOrEditEquipmentPage");
         });

        public DelegateCommand<Equipment> GoToEditPageCommand => new DelegateCommand<Equipment>(async (Equipment equipment) =>
        {
            await _navigationService.NavigateAsync($"AddOrEditEquipmentPage?id={equipment.Id}");
        }); 

        public DelegateCommand SelectionChangedCommand => new DelegateCommand(async () =>
        {
            await _navigationService.NavigateAsync($"ViewEquipmentPage?id={_selectedEquipment.Id}");
            _selectedEquipment = null;
        });


        public DelegateCommand RefreshCommand => new DelegateCommand(() =>
        {
            IsRefreshing = true;
            loadItems();
            IsRefreshing = false;
            RaisePropertyChanged(nameof(IsRefreshing));
        });

        public DelegateCommand PageOnAppearingCommand => new DelegateCommand(() =>
        {
            loadItems();
        });
    }
}
