using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AppCenter.Analytics;
using ShoppingCart.DataService;
using ShoppingCart.Models;
using ShoppingCart.ViewModels.Detail;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using TypeLocator = ShoppingCart.MockDataService.TypeLocator;

namespace ShoppingCart.Views.Detail
{
    /// <summary>
    /// The Detail page.
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailPage
    {

        Stopwatch stopWatch;
        string locationData = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailPage" /> class.
        /// </summary>
        public DetailPage(Product selectedProduct)
        {
            InitializeComponent();
            stopWatch = new Stopwatch();
            var catalogDataService = App.MockDataService
                ? TypeLocator.Resolve<ICatalogDataService>()
                : DataService.TypeLocator.Resolve<ICatalogDataService>();
            var cartDataService = App.MockDataService
                ? TypeLocator.Resolve<ICartDataService>()
                : DataService.TypeLocator.Resolve<ICartDataService>();
            var wishlistDataService = App.MockDataService
                ? TypeLocator.Resolve<IWishlistDataService>()
                : DataService.TypeLocator.Resolve<IWishlistDataService>();
            BindingContext = new DetailPageViewModel(catalogDataService, cartDataService, wishlistDataService,
                selectedProduct);
        }

        /// <summary>
        /// Invoke when view is appear.
        /// </summary>
        protected override void OnAppearing()
        {
            stopWatch.Restart();
            base.OnAppearing();
        }

        /// <summary>
        /// Invoked when view size is changed.
        /// </summary>
        /// <param name="width">The Width</param>
        /// <param name="height">The Height</param>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width > height)
                Rotator.ItemTemplate = (DataTemplate)Resources["LandscapeTemplate"];
            else
                Rotator.ItemTemplate = (DataTemplate)Resources["PortraitTemplate"];
        }

        /// <summary>
        /// Invoke when view is disappear.
        /// </summary>
        protected override void OnDisappearing()
        {
            stopWatch.Stop();
            Analytics.TrackEvent("Product detail page loaded",
                       new Dictionary<string, string> { { "Product Name", (BindingContext as DetailPageViewModel).ProductDetail.Name }, { "Time Spend", stopWatch.Elapsed.ToString() } });
            base.OnDisappearing();
        }
    }
}