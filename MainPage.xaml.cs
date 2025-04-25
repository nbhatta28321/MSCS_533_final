using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps; // Correct namespace for Position and Pin
using LocationTrackingApp.Models;
using System.Linq;
using System.Collections.Generic;
using SQLite;
using Microsoft.Maui.Maps; // Ensure this namespace is here

namespace LocationTrackingApp
{
    public partial class MainPage : ContentPage
    {
        private GeolocationRequest _request;
        private Location _lastKnownLocation;

        public MainPage()
        {
            InitializeComponent();
            _request = new GeolocationRequest(GeolocationAccuracy.Best);
            StartTrackingLocation();
        }

        private async void StartTrackingLocation()
        {
            try
            {
                var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permission Denied", "Location permission is required", "OK");
                    return;
                }

                var location = await Geolocation.GetLocationAsync(_request);
                if (location != null)
                {
                    _lastKnownLocation = location;
                    SaveLocationToDatabase(location);
                    UpdateMap(location);
                }

                Device.StartTimer(TimeSpan.FromSeconds(10), () =>
                {
                    StartTrackingLocation(); 
                    return true;
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to get location: {ex.Message}", "OK");
            }
        }

        private void SaveLocationToDatabase(Location location)
        {
            var newLocation = new LocationModel
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Timestamp = DateTime.Now
            };

            App.Database.Insert(newLocation); // Insert location data into SQLite
        }

        private void UpdateMap(Location location)
        {
            var position = new Microsoft.Maui.Maps.Position(location.Latitude, location.Longitude); // Correct Position class reference

            MainMap.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(1))); // Correct MapSpan usage

            var pin = new Microsoft.Maui.Controls.Maps.Pin
            {
                Label = "You are here",
                Position = position // Correct Pin usage
            };

            MainMap.Pins.Add(pin);
        }
    }
}
