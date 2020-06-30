using Prism;
using Prism.Ioc;
using XamConsultingTest.ViewModels;
using XamConsultingTest.Views;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials.Implementation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamConsultingTest.Helpers;
using System;
using System.IO;
using MonkeyCache.SQLite;
using MonkeyCache;
using Plugin.Media;

namespace XamConsultingTest
{
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            Device.SetFlags(new[]
           {
                "Shapes_Experimental"
            });

            var navigate = await NavigationService.NavigateAsync("NavigationPage/EquipmentPage");
            if (!navigate.Success)
            {
                throw new Exception(navigate.Exception.Message);
            }
        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<EquipmentPage, EquipmentPageViewModel>();
            containerRegistry.RegisterForNavigation<AddOrEditEquipmentPage, AddOrEditEquipmentPageViewModel>();
            containerRegistry.RegisterForNavigation<ViewEquipmentPage, ViewEquipmentPageViewModel>();
            containerRegistry.AutoRegisterByInterfaceName("IGetAll");
            containerRegistry.AutoRegisterByInterfaceName("IPost");
            containerRegistry.AutoRegisterByInterfaceName("IPut");
            containerRegistry.AutoRegisterByInterfaceName("IGet");
            Barrel.Create(OfflineDatabasePath());
            Barrel.ApplicationId = "LocalDb";

            containerRegistry.RegisterInstance(CrossMedia.Current);
            containerRegistry.RegisterInstance(Barrel.Current);
        }
        private string OfflineDatabasePath()
        {
            if (Device.RuntimePlatform == Device.iOS)
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library");

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
        }
    }
}

