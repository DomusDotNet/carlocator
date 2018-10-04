using System;
using System.Globalization;
using Xamarin.Essentials;

namespace TPCWare.CarLocator.Models
{
    public static class Settings
    {
        private static string SharedName => "CarLocator";

        private static string LastCarPosition
        {
            get => Preferences.Get("LastCarPosition", String.Empty, SharedName);
            set => Preferences.Set("LastCarPosition", value, SharedName);
        }

        public static Location GetCarLocation()
        {
            Location location = null;
            string carLocation = LastCarPosition;
            if(!string.IsNullOrWhiteSpace(carLocation))
            {
                var values = carLocation.Split(',');
                double latitude = double.TryParse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) ? lat : 0.0;
                double longitude = double.TryParse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double lon) ? lon : 0.0;
                DateTimeOffset timestampUtc = DateTimeOffset.TryParse(values[2], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,out DateTimeOffset utc) ? utc : DateTimeOffset.Now;
                location = new Location(latitude, longitude, timestampUtc);
            }

            return location;
        }

        public static void SetCarLocation(Location location)
        {
            LastCarPosition = $"{location.Latitude.ToString(CultureInfo.InvariantCulture)},{location.Longitude.ToString(CultureInfo.InvariantCulture)},{location.TimestampUtc.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}