using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TPCWare.CarLocator.Models;
using Xamarin.Essentials;

namespace TPCWare.CarLocator.ViewModels
{
    public class MainViewModel: BaseViewModel
    {
        private Exception gpsException;
        private CancellationTokenSource cts;
        private string resultMessage;

        #region Properties

        public bool CarLocationAvailable => (CarLocation != null);

        public Location CarLocation => Settings.GetCarLocation();

        #endregion

        public MainViewModel() : base()
        {
        }

        public async Task SetNewCarLocation()
        {
            Location location = await GetGeoLocationAsync();
            if (location == null)
            {
                // Show error
                // TODO
            }
            else
            {
                // Update car location
                Settings.SetCarLocation(location);

                // Update MapView
                // TODO
            }
        }

        public async Task<Location> GetGeoLocationAsync()
        {
            Location location = null;

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
            }

            return location;
        }
    }
}
