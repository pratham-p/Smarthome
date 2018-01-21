using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Net;
using Android.Net.Wifi;
using System.Threading.Tasks;

namespace smarthome
{
    public class ConnectionManager
    {
        public bool IsOnline(Context appContext, string homeNetworkName)
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)appContext.GetSystemService(Context.ConnectivityService);
            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
            bool isOnline = (activeConnection != null) && activeConnection.IsConnected;
            if (isOnline)
            {
                NetworkInfo netInfo = connectivityManager.ActiveNetworkInfo; //.GetNetworkInfo(ConnectivityType.Wifi);
                if (netInfo.Type == ConnectivityType.Wifi && netInfo.IsConnectedOrConnecting)
                {
                    WifiManager wifiManager = (WifiManager)appContext.GetSystemService(Context.WifiService);

                    if (wifiManager.ConnectionInfo != null)
                    {
                        string wifiSSID = wifiManager.ConnectionInfo.SSID.Trim('"');
                        if (wifiSSID.Equals(homeNetworkName, StringComparison.CurrentCultureIgnoreCase))
                            return true;
                    }
                }
            }
            return false;
        }
    }
}