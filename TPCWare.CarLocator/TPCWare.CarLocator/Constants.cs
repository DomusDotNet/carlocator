using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace TPCWare.CarLocator
{
    public static class Constants
    {
        public static readonly GeolocationAccuracy GPS_ACCURACY = GeolocationAccuracy.Best;
        public static readonly int NEW_REQUEST_MINIMUM_SECONDS = 60;
    }
}
