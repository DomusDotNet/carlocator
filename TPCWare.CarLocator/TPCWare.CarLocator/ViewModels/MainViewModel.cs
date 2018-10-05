using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TPCWare.CarLocator.Models;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;

namespace TPCWare.CarLocator.ViewModels
{
    public class MainViewModel: BaseViewModel
    {
        private Exception gpsException;
        private CancellationTokenSource cts;
        private string resultMessage;

        #region Properties

        private Location carLocation = Settings.GetCarLocation();
        public Location CarLocation
        {
            get => carLocation;
            set => SetProperty(ref carLocation, value, onChanged: () =>
            {
                OnPropertyChanged(nameof(CarLocationAvailable));
                OnPropertyChanged(nameof(CarPositionGeoCoordinates));
                OnPropertyChanged(nameof(CarPositionTimestamp));
            });
        }

        public bool CarLocationAvailable => (CarLocation != null);

        public string CarPositionGeoCoordinates => $"{CarLocation?.Latitude.ToString("###.00")}, {CarLocation?.Longitude.ToString("###.00")}";
        public string CarPositionTimestamp => $"{CarLocation?.TimestampUtc.ToLocalTime().ToString("dd/MM/yy hh:mm")}";

        #endregion

        public MainViewModel() : base()
        {
        }

        public async Task SetNewCarLocationAsync(Map map)
        {
            Location location = await GetGeoLocationAsync();
            if (location != null)
            {
                // Update car location
                await DisplayAlertAsync(location.TimestampUtc.ToString());
                Settings.SetCarLocation(location);
                CarLocation = location;

                // Update Map
                await UpdateMapAsync(map);
            }
        }

        public async Task UpdateMapAsync(Map map)
        {
            Location userLocation = await GetGeoLocationAsync();
            Location carLocation = Settings.GetCarLocation();

            var km = (userLocation == null || carLocation == null)
                ? 1.0
                : Location.CalculateDistance(userLocation, carLocation, DistanceUnits.Kilometers);

            km = Math.Min(0.5, km);

            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(userLocation.Latitude, userLocation.Longitude), new Distance(1000 * km)));

            if (carLocation != null)
            {
                var carPin = new Pin()
                {
                    Type = PinType.SavedPin,
                    Position = new Position(carLocation.Latitude, carLocation.Longitude),
                    Label = "La mia macchina"
                };

                map.Pins.Clear();
                map.Pins.Add(carPin);
            }
        }

        public async Task<Location> GetGeoLocationAsync()
        {
            Location location = null;
            if (IsNotBusy)
            {
                IsBusy = true;
                try
                {
                    var request = new GeolocationRequest(Constants.GPS_ACCURACY);
                    cts = new CancellationTokenSource();
                    location = await Geolocation.GetLocationAsync(request, cts.Token);
                }
                catch (FeatureNotSupportedException fnsEx)
                {
                    // Handle not supported on device exception
                    gpsException = fnsEx;
                    resultMessage = "GPS non disponibile in questo device.";
                }
                catch (PermissionException pEx)
                {
                    // Handle permission exception
                    gpsException = pEx;
                    resultMessage = "Devi attivare il GPS per vedere la lista.";
                }
                catch (Exception ex)
                {
                    // Unable to get location
                    gpsException = ex;
                    resultMessage = $"Impossibile recuperare la tua posizione dal GPS. ({ex.Message})";
                }
                finally
                {
                    cts.Dispose();
                    cts = null;
                    IsBusy = false;
                }
            }
            else
            {
                resultMessage = $"Una precedente richiesta posizione dal GPS è già in corso.";
            }

            if (location == null)
            { 
                // Show error
                await DisplayAlertAsync(resultMessage);
            }

            return location;
        }

    }
}
