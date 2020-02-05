using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AppCenter.Analytics;
using ShoppingCart.DataService;
using ShoppingCart.ViewModels.Catalog;
using Syncfusion.ListView.XForms;
using Syncfusion.XForms.ComboBox;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using TypeLocator = ShoppingCart.MockDataService.TypeLocator;

namespace ShoppingCart.Views.Catalog
{
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProductHomePage : ContentPage
    {
        Stopwatch stopWatch = new Stopwatch();


        public ProductHomePage()
        {
            InitializeComponent();
            var productHomeDataService = App.MockDataService
                ? TypeLocator.Resolve<IProductHomeDataService>()
                : DataService.TypeLocator.Resolve<IProductHomeDataService>();
            var catalogDataService = App.MockDataService
                ? TypeLocator.Resolve<ICatalogDataService>()
                : DataService.TypeLocator.Resolve<ICatalogDataService>();
            BindingContext = new ProductHomePageViewModel(productHomeDataService, catalogDataService);
        }

        protected override void OnAppearing()
        {
            stopWatch.Restart();
            base.OnAppearing();
        }


        protected override void OnDisappearing()
        {
            stopWatch.Stop();
            Analytics.TrackEvent("Home page loaded",
                       new Dictionary<string, string> { { "Time Spend", stopWatch.Elapsed.ToString() } });
            base.OnDisappearing();
        }
    }
}