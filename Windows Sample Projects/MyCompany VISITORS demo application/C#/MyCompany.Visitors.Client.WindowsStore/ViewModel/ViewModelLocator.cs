namespace MyCompany.Visitors.Client.WindowsStore.ViewModel
{
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Ioc;
    using Microsoft.Practices.ServiceLocation;
    using MyCompany.Visitors.Client.WindowsStore.Services.Dispatcher;
    using MyCompany.Visitors.Client.WindowsStore.Services.Messages;
    using MyCompany.Visitors.Client.WindowsStore.Services.Navigation;
    using MyCompany.Visitors.Client.WindowsStore.Services.NFC;
    using MyCompany.Visitors.Client.WindowsStore.Services.SampleData;
    using MyCompany.Visitors.Client.WindowsStore.Services.Storage;
    using MyCompany.Visitors.Client.WindowsStore.Services.Tiles;
    using MyCompany.Visitors.Client.WindowsStore.Settings;

    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            Register();
        }

        /// <summary>
        /// Register all the Services and ViewModels.
        /// </summary>
        public void Register()
        {
            SimpleIoc.Default.Reset();
            SimpleIoc.Default.Register<IStorageService, StorageService>();
            SimpleIoc.Default.Register<IMessageService,MessageService>();
            SimpleIoc.Default.Register<ITilesService,TilesService>();
            SimpleIoc.Default.Register<INavigationService, NavigationService>(true);
            SimpleIoc.Default.Register<IDispatcherService, DispatcherService>(true);
            SimpleIoc.Default.Register<INfcService, NfcService>(true);
            SimpleIoc.Default.Register<ISampleDataService, SampleDataService>(true);
            SimpleIoc.Default.Register<IMyCompanyClient>(() =>
            {
                if (AppSettings.TestMode)
                {
                    string api = string.Format("{0}noauth/", AppSettings.ApiUri.ToString());
                    string token = "TestToken";
                    return new MyCompanyClient(api, token);
                }
                if (ViewModelBase.IsInDesignModeStatic)
                    return null;

                return new MyCompanyClient(AppSettings.ApiUri.ToString(), AppSettings.SecurityToken);
            });
            SimpleIoc.Default.Register<VMAuthentication>();
            SimpleIoc.Default.Register<VMMainPage>();
            SimpleIoc.Default.Register<VMConfigurationPanel>();
            SimpleIoc.Default.Register<VMAuthenticatedEmployee>();
            SimpleIoc.Default.Register<VMCropPicture>();
            SimpleIoc.Default.Register<VMVisitsListingPage>();
            SimpleIoc.Default.Register<VMVisitDetailPage>();
            SimpleIoc.Default.Register<VMNewVisitPage>();
            SimpleIoc.Default.Register<VMSearchVisitorPage>();
            SimpleIoc.Default.Register<VMSearchEmployeePage>();
            SimpleIoc.Default.Register<VMNewVisitorPage>();
        }

        /// <summary>
        /// Main page viewmodel instance.
        /// </summary>
        public VMMainPage MainViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<VMMainPage>();
            }
        }

        /// <summary>
        /// Authentication viewmodel instance.
        /// </summary>
        public VMAuthentication AuthViewModel
        {
            get { return ServiceLocator.Current.GetInstance<VMAuthentication>(); }
        }

        /// <summary>
        /// Configuration panel viewmodel instance.
        /// </summary>
        public VMConfigurationPanel ConfigurationViewModel
        {
            get { return ServiceLocator.Current.GetInstance<VMConfigurationPanel>(); }
        }

        /// <summary>
        /// Authenticated employee control viewmodel instance.
        /// </summary>
        public VMAuthenticatedEmployee AuthEmployeeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<VMAuthenticatedEmployee>(); }
        }

        /// <summary>
        /// Cropping picture viewmodel instance.
        /// </summary>
        public VMCropPicture CropPictureViewModel
        {
            get { return new VMCropPicture(new StorageService()); }
        }

        /// <summary>
        /// Visits list viewmodel instance.
        /// </summary>
        public VMVisitsListingPage VisitsListViewModel
        {
            get { return ServiceLocator.Current.GetInstance<VMVisitsListingPage>(); }
        }

        /// <summary>
        /// Visit details viewmodel instance.
        /// </summary>
        public VMVisitDetailPage VisitDetailViewModel
        {
            get { return ServiceLocator.Current.GetInstance<VMVisitDetailPage>(); }
        }

        /// <summary>
        /// New visit viewmodel instance.
        /// </summary>
        public VMNewVisitPage NewVisitViewModel
        {
            get { return ServiceLocator.Current.GetInstance<VMNewVisitPage>(); }
        }

        /// <summary>
        /// Search visitor viewmodel instance.
        /// </summary>
        public VMSearchVisitorPage SearchVisitorViewModel
        {
            get { return ServiceLocator.Current.GetInstance<VMSearchVisitorPage>(); }
        }

        /// <summary>
        /// Search employee viewmodel instance.
        /// </summary>
        public VMSearchEmployeePage SearchEmployeeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<VMSearchEmployeePage>(); }
        }

        /// <summary>
        /// New visitor viewmodel instance.
        /// </summary>
        public VMNewVisitorPage NewVisitorViewModel
        {
            get { return ServiceLocator.Current.GetInstance<VMNewVisitorPage>(); }
        }

        /// <summary>
        /// Navigation service instance.
        /// </summary>
        public INavigationService NavService
        {
            get { return ServiceLocator.Current.GetInstance<INavigationService>(); }
        }

        /// <summary>
        /// Nfc service instance.
        /// </summary>
        public INfcService NfcService
        {
            get { return ServiceLocator.Current.GetInstance<INfcService>(); }
        }

        /// <summary>
        /// Client service instance.
        /// </summary>
        public IMyCompanyClient MyCompanyClientService
        {
            get { return ServiceLocator.Current.GetInstance<IMyCompanyClient>(); }
        }
    }
}