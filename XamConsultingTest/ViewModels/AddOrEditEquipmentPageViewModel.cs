using Newtonsoft.Json.Schema;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Toast;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using XamConsultingTest.Helpers;
using XamConsultingTest.Interfaces;
using XamConsultingTest.Models;

namespace XamConsultingTest.ViewModels
{
    public class AddOrEditEquipmentPageViewModel : ViewModelBase,INavigationAware
    {
        private readonly INavigationService _navigationService;
        private readonly IPost<Equipment> _equipmentPostService;
        private readonly IGet<Equipment> _equipmentGetService;
        private readonly IPut<Equipment> _equipmentPutService;
        private bool _isAddForm;
        
        private Equipment _equipment = new Equipment();
        public string Name 
        {
            get => _equipment.Name ??"";
            set
            {
                _equipment.Name = value;
            }
        }
        public string Comments
        {
            get => _equipment.Comments ?? "";
            set
            {
                _equipment.Comments = value;
            }
        }

        public string PreviewImageSource 
        {
            get => _equipment.PhotoUri ?? "ic_defaultImage.png";
            set
            {
                _equipment.PhotoUri = value;
            }
        }
        public AddOrEditEquipmentPageViewModel(INavigationService navigationService,
            IPost<Equipment> equipmentPostService,
            IGet<Equipment> equipmentGetService,
            IPut<Equipment> equipmentPutService)
            : base(navigationService)
        {
            this._navigationService = navigationService;
            this._equipmentPostService = equipmentPostService;
            this._equipmentGetService = equipmentGetService;
            this._equipmentPutService = equipmentPutService;
        }
        public DelegateCommand CaptureEquipmentPhotoCommand => new DelegateCommand( async() =>
        {
            await CrossMedia.Current.Initialize();

            if(!CrossMedia.Current.IsTakePhotoSupported)
                await App.Current.MainPage.DisplayAlert("Info", "Photo Capture Not Supported", "Ok");

            var equipmentPhoto = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
            {
                Directory = "XamConsultingExamImages",
                Name = $"{DateTime.Now}.jpg",
                MaxWidthHeight = 350,
                PhotoSize = PhotoSize.MaxWidthHeight,
                CompressionQuality = 92
            });
            previewPhoto(equipmentPhoto);
        });
        public DelegateCommand UploadEquipmentPhotoCommand => new DelegateCommand(async () =>
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
                await App.Current.MainPage.DisplayAlert("Info", "Photo Picking Not Supported", "Ok");

            var equipmentPhoto = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions()
            {
                ModalPresentationStyle = MediaPickerModalPresentationStyle.FullScreen,
                MaxWidthHeight = 350,
                PhotoSize = PhotoSize.MaxWidthHeight,
                CompressionQuality = 92
            });
            previewPhoto(equipmentPhoto);
        });

        private void previewPhoto(MediaFile equipmentPhoto)
        {
            if (equipmentPhoto != null)
            {
                var dataUri = ImageHelper.ToBase64(equipmentPhoto.GetStream());
                _equipment.PhotoUri = dataUri;
                RaisePropertyChanged(nameof(PreviewImageSource));
            }
            else
                RaisePropertyChanged(nameof(PreviewImageSource));
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            Title = (parameters.Count > 0) ? "Edit Equipment/Tool" : "Add Equipment/Tool";
            if (parameters.Count>0)
            {
                var equipment = _equipmentGetService.Get(parameters.GetValue<int>("id"));
                if (equipment == null)
                    await App.Current.MainPage.DisplayAlert("Info", "Equipment not found", "Ok");
                else
                {
                    _equipment = equipment;
                    RaisePropertyChanged(nameof(PreviewImageSource));
                    RaisePropertyChanged(nameof(Name));
                    RaisePropertyChanged(nameof(Comments));
                }
                _isAddForm = false; 
            }
            else
                _isAddForm = true;
        }

        public DelegateCommand SubmitCommand => new DelegateCommand(async () =>
        {
            var transactionType = (_isAddForm) ?"add":"edit";;
            var validationMessage = ValidationCheck();
            if(validationMessage!=string.Empty)
                CrossToastPopUp.Current.ShowToastMessage(validationMessage);
            else
            {
                var isToAddOrUpdateEquipment = await App.Current.MainPage.DisplayAlert("Confirmation", $"Do you really want to {transactionType} this equipment/tool?", "Yes","No");
                if(isToAddOrUpdateEquipment)
                {
                    if (_isAddForm)
                        _equipmentPostService.Post(_equipment);
                    else
                        _equipmentPutService.Put(_equipment);

                    await App.Current.MainPage.DisplayAlert("Info", $"Successfully {transactionType}ed equipment/tools", "Ok");
                    await _navigationService.GoBackAsync();
                }
            }
        });
        private string ValidationCheck()
        {
            if(_equipment.Name==null || _equipment.Name.Trim() == string.Empty)
                return "Equipment Name is Required";
            else if (_equipment.Comments == null || _equipment.Comments.Trim() == string.Empty)
                return "Equipment Comments is Required";
            if (_equipment.PhotoUri == null || _equipment.Name.Trim() == string.Empty)
                return "Equipment Photo is Required";
            return string.Empty;
        }
    }
}
