using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Net;
using Android.Net.Wifi;

using smarthome.DataService.Controllers;
using smarthome.DataService.Lib;
using smarthome.DataService.Interfaces;
using smarthome.DataService.Services;

namespace smarthome.Receivers
{
    [BroadcastReceiver(Name = "smarthome.Receivers.NetworkStatusBroadcastReceiver",
        Label = "Receiver for Network change events",
        Enabled = true)]
    [IntentFilter(new[] { Android.Net.ConnectivityManager.ConnectivityAction })] //android.net.conn.CONNECTIVITY_CHANGE
    public class NetworkStatusBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            #region Commented Code
            //if (intent != null)
            //{
            //    Toast.MakeText(context, "ACTION: " + intent.Action, ToastLength.Short).Show();
            //    Toast.MakeText(context, "Extras", ToastLength.Short).Show();

            //    foreach (String key in intent.Extras.KeySet())
            //    {
            //        Object value = intent.Extras.Get(key);
            //        Toast.MakeText(context, String.Format("{0} {1} ({2})", key, value.ToString(), value.GetType().Name), ToastLength.Short).Show();
            //        System.Threading.Thread.Sleep(2000);
            //    }
            //}
            #endregion

            // Initialize SharedPrefService
            ISharedPreferenceService prefService = new SharedPreferenceService(Constants.PrefFileName);

            // Read Network and Server name from local storage
            string storedNetworkName = prefService.GetSetting<string>(Constants.PrefHomeNetworkKey);
            string storedServerName = prefService.GetSetting<string>(Constants.PrefServerNameKey);

            // Check network name is registered
            if (!string.IsNullOrWhiteSpace(storedNetworkName))
            {
                // Get connection information
                IConnectionService connection = ConnectionService.Instance;
                if (connection.IsConnected)
                {
                    // if wifi is connected
                    if (connection.Type == ConnectionType.Wifi)
                    {
                        if (connection.ConnectionInfo.IsConnected)
                        {
                            // Get wifi network SSID
                            string networkName = connection.ConnectionInfo.ExtraInfo.Trim('"');

                            // Check if registered network is connected by matching SSID
                            if (networkName.Equals(storedNetworkName, StringComparison.CurrentCultureIgnoreCase) && IsServerOnline(storedServerName))
                            {
                                // Turn on Entrance Light
                                Task<bool> success = TurnOnEntranceLight(storedServerName);

                                //Send Notification
                                new MyAppNotification(context).SendNotification("Smart Home", "Entrance lights ON", Resource.Drawable.Icon, 10);
                            }
                        }
                    }
                }
            }

            #region Commented Old Code
            /*
            bool noConnectivity = false;
            if (intent.Extras.ContainsKey(ConnectivityManager.ExtraNoConnectivity))
                noConnectivity = intent.GetBooleanExtra(ConnectivityManager.ExtraNoConnectivity, false);

            if (!noConnectivity)
            {
                if (intent.GetIntExtra(ConnectivityManager.ExtraNetworkType, -1) == (int)ConnectivityType.Wifi)
                {
                    if (intent.Extras.ContainsKey(ConnectivityManager.ExtraNetworkInfo))
                    {
                        NetworkInfo netInfo = intent.GetParcelableExtra(ConnectivityManager.ExtraNetworkInfo) as NetworkInfo;
                        if (netInfo.Type == ConnectivityType.Wifi
                            && netInfo.GetState() == NetworkInfo.State.Connected
                            && netInfo.IsAvailable)
                        {
                            if (!string.IsNullOrWhiteSpace(netInfo.ExtraInfo))
                            {
                                string networkName = netInfo.ExtraInfo.Trim('"');

                                if (networkName.Equals(storedNetworkName, StringComparison.CurrentCultureIgnoreCase) && CheckAppIsOnline(storedServerName))
                                {
                                    Task<bool> success = TurnOnEntranceLight(storedServerName);

                                    //Send Notification
                                    new MyAppNotification(context).SendNotification("Smart Home", "Living Room lights ON", Resource.Drawable.Icon, 10);
                                }
                            }
                        }
                    }
                }
            }


        //Toast.MakeText(context, "Connected: " + isConnected.ToString(), ToastLength.Short).Show();

        //if (isConnected)
        //{
        //    //Toast.MakeText(context, intent.GetIntExtra(ConnectivityManager.ExtraNetworkType, 0).ToString(), ToastLength.Short).Show();

        //    //ConnectivityType networkType = (ConnectivityType)intent.GetIntExtra(ConnectivityManager.ExtraNetworkType, 0);
        //    //if (networkType == ConnectivityType.Wifi)
        //    {
        //        //Toast.MakeText(context, networkType.ToString(), ToastLength.Short).Show();

        //        Network network = (Network)intent.GetParcelableExtra(ConnectivityManager.ExtraNetwork);

        //        Toast.MakeText(context, network == null ? "Network NULL" : "Network YES", ToastLength.Short).Show();

        //        ConnectivityManager connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);

        //        Toast.MakeText(context, connectivityManager == null ? "CM NULL" : "CM YES", ToastLength.Short).Show();

        //        NetworkInfo netInfo = connectivityManager.GetNetworkInfo(network);

        //        Toast.MakeText(context, netInfo == null ? "NI NULL" : "NI YES", ToastLength.Short).Show();

        //        if (netInfo.IsConnected)
        //        {
        //            Toast.MakeText(context, netInfo.IsConnected.ToString(), ToastLength.Short).Show();

        //            WifiManager wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);
        //            if (wifiManager.ConnectionInfo != null)
        //            {
        //                Toast.MakeText(context, wifiManager.ConnectionInfo.SSID, ToastLength.Short).Show();

        //                string wifiSSID = wifiManager.ConnectionInfo.SSID.Trim('"');
        //                if (wifiSSID.Equals("Home007", StringComparison.CurrentCultureIgnoreCase))
        //                {

        //                }
        //            }
        //        }
        //    }
        //}
        */
            #endregion
        }

        private async Task<bool> TurnOnEntranceLight(string serverName)
        {
            // Turn on entrance light
            return await new LightController(serverName).TriggerLight("entranceLight", "on");
        }

        private bool IsServerOnline(string serverName)
        {
            // Check if 
            if (!string.IsNullOrWhiteSpace(serverName))
                return new LightController(serverName).PingTest();
            else
                return false;
        }
    }
}