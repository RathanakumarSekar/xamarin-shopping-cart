using System.Collections.Generic;
using ShoppingCart.DataService;
using ShoppingCart.Mapping;
using ShoppingCart.Models;
using ShoppingCart.Views.ErrorAndEmpty;
using ShoppingCart.Views.Home;
using ShoppingCart.Views.Onboarding;
using Xamarin.Essentials;
using Xamarin.Forms;
using TypeLocator = ShoppingCart.MockDataService.TypeLocator;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;

namespace ShoppingCart
{
    public partial class App : Application
    {
        public static string BaseUri = "Your Web API";

        string locationData = string.Empty;


        public static bool MockDataService = true;

        public App()
        {
            InitializeComponent();


            if (MockDataService)
            {
                TypeLocator.Start();
                MainPage = new NavigationPage(new OnBoardingAnimationPage());
            }
            else
            {
                ListenNetworkChanges();
                if (!SQLiteDatabase.Shared.Initialized) SQLiteDatabase.Shared.Init();

                DataService.TypeLocator.Start();
                MapperConfig.Config();
                GetUserInfo();
            }
        }

        public static string BaseImageUrl { get; } =
            "https://cdn.syncfusion.com/essential-ui-kit-for-xamarin.forms/common/uikitimages/";

        public static int CurrentUserId { get; set; }
        public static string UserEmailId { get; set; }

        public static string UserName { get; set; }

        private void GetUserInfo()
        {
            var userInfo = SQLiteDatabase.Shared.GetUserInfo();
            if (userInfo != null)
            {
                CurrentUserId = userInfo.UserId;
                UserEmailId = userInfo.EmailId;
                UserName = userInfo.UserName;
                MainPage = new NavigationPage(new HomePage());
            }
            else
            {
                MainPage = new NavigationPage(new OnBoardingAnimationPage());
            }
        }

        private static void ListenNetworkChanges()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        private static void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            CheckInternet();
        }


        private static void CheckInternet()
        {
            var onErrorPage = false;
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                onErrorPage = true;
                Current.MainPage.Navigation.PushAsync(new NoInternetConnectionPage());
            }
            else if (onErrorPage)
            {
                Current.MainPage.Navigation.PopAsync();
                onErrorPage = false;
            }
        }

        private async System.Threading.Tasks.Task GetLocationDetailsAsync()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    locationData = $"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}";
                }
            }
            catch (FeatureNotSupportedException e)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException e)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException e)
            {
                // Handle permission exception
            }
            catch (Exception e)
            {
                // Unable to get location
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            AppCenter.Start("android={Your Android App secret here};" +
                  "uwp={Your UWP App secret here};" +
                  "ios={Your iOS App secret here};",
                  typeof(Analytics), typeof(Crashes));
            Analytics.SetEnabledAsync(true);
            GetLocationDetailsAsync();

            Analytics.TrackEvent("Application Started ",
                       new Dictionary<string, string> { { "Location", locationData }, { "OS Version", Xamarin.Essentials.DeviceInfo.VersionString }, { "Device Model", Xamarin.Essentials.DeviceInfo.Manufacturer + Xamarin.Essentials.DeviceInfo.Model } });
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}