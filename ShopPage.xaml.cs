using BlagaAntoniaLab7.Models;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel; 
using Plugin.LocalNotification;        

namespace BlagaAntoniaLab7;

public partial class ShopPage : ContentPage
{
    public ShopPage()
    {
        InitializeComponent();
    }

    async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        var shop = (Shop)BindingContext;
        await App.Database.SaveShopAsync(shop);
        await Navigation.PopAsync();
    }

    
    async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var shop = (Shop)BindingContext;
        await App.Database.DeleteShopAsync(shop);
        await Navigation.PopAsync();
    }
  

    async void OnShowMapButtonClicked(object sender, EventArgs e)
    {
        var shop = (Shop)BindingContext;
        var address = shop.Adress;
        var options = new MapLaunchOptions { Name = "Magazinul meu preferat" };

        Location shoplocation = null;

        try
        {
           
            var locations = await Geocoding.GetLocationsAsync(address);
            shoplocation = locations?.FirstOrDefault();
        }
        catch (Exception)
        {
            
        }

        
        if (shoplocation == null)
        {
            shoplocation = new Location(46.7712, 23.6236);
        }

        if (shoplocation != null)
        {
            
            Location myLocation = null;
            try
            {
                myLocation = await Geolocation.GetLocationAsync();
            }
            catch { }

            
            if (myLocation == null)
            {
                myLocation = new Location(46.7731796289, 23.6213886738);
            }

            var distance = myLocation.CalculateDistance(shoplocation, DistanceUnits.Kilometers);

            if (distance < 5)
            {
                var request = new NotificationRequest
                {
                    NotificationId = 100,
                    Title = "Ai de facut cumparaturi in apropiere!",
                    Description = address,
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddSeconds(1)
                    }
                };
                LocalNotificationCenter.Current.Show(request);
            }

            await Map.OpenAsync(shoplocation, options);
        }
    }
}