using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net;
using System.Threading.Tasks;
using Android.Net;
using Android.Util;
using Android.Net.Wifi;
using smarthome.DataService.Controllers;
using Android.Support.V4.Widget;
using Android.Content.PM;

namespace smarthome
{
    [Activity(Label = "@string/ApplicationName", 
        ScreenOrientation = ScreenOrientation.Portrait, 
        MainLauncher = false, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        string TAG = "Smartlights";
        bool isHomeNetworkConnected = false;
        int wifiRed = Resource.Drawable.wifi_red;
        int wifiGreen = Resource.Drawable.wifi_green;

        ImageView wifiImage;
        SwipeRefreshLayout swipeRefreshLayout;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            wifiImage = FindViewById<ImageView>(Resource.Id.wifiImage);
            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.mainSwipeLayout);

            // set color scheme for swipe refresh
            swipeRefreshLayout.SetColorScheme(
                Android.Resource.Color.HoloBlueBright,
                Android.Resource.Color.HoloBlueDark,
                Android.Resource.Color.HoloRedDark,
                Android.Resource.Color.HoloGreenLight);

            // Attach Refresh event
            swipeRefreshLayout.Refresh += SwipeRefreshLayout_Refresh;

            // Check home network and update ui
            if (checkHomeNetworkConnection())
            {
                isHomeNetworkConnected = true;
                setWifiImage(wifiGreen);
            }

            StartActivity(typeof(HomeScreen));
        }

        #region Commented (Ex: Asynchronous task - Background worker method)
        //private void SwipeRefreshLayout_Refresh(object sender, EventArgs e)
        //{
        //    System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();
        //    worker.DoWork += Worker_DoWork;
        //    worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        //    worker.RunWorkerAsync();
        //}

        //private void Worker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        //{
        //    // Add logic to run after the worker has completed the work asynchronously
        //    RunOnUiThread(() => { swipeRefreshLayout.Refreshing = false; });


        //    //Not exacctly sure about BeginInvokeOnMainThread code.
        //    //Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
        //    //{
        //    //    //Code to run on UI (main) thread
        //    //});
        //}

        //private void Worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        //{
        //    // Add logic to run Asynchronously
        //    System.Threading.Thread.Sleep(5000);
        //}
        #endregion

        /// <summary>
        /// Delegate method for SwipeRefreshLayout Refresh event
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">Event args</param>
        private async void SwipeRefreshLayout_Refresh(object sender, EventArgs e)
        {
            // Asynchronous Task - Async-Await method
            try
            {
                await Task.Run(() =>
               {
                   // Check home network connection
                   isHomeNetworkConnected = checkHomeNetworkConnection();
                   System.Threading.Thread.Sleep(3000);
               });

                // Run on UI thread
                RunOnUiThread(() =>
                {
                    // Update ui
                    if (isHomeNetworkConnected)
                        setWifiImage(wifiGreen);
                    else
                        setWifiImage(wifiRed);

                    swipeRefreshLayout.Refreshing = false;
                });
            }
            catch (Exception ex)
            {
                Log.Error($"{TAG}::{ex.GetType().Name}", $"{ex.Message}::{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Check home network connection
        /// </summary>
        /// <returns>IsConnected?</returns>
        private bool checkHomeNetworkConnection()
        {
            return new ConnectionManager().IsOnline(ApplicationContext, "Home007");
        }

        /// <summary>
        /// Set Wifi connection image on ui
        /// </summary>
        /// <param name="resourceId">Image resource id</param>
        private void setWifiImage(int resourceId)
        {
            wifiImage.SetImageResource(resourceId);
        }

        /// <summary>
        /// Button click event
        /// </summary>
        /// <param name="view">Triggered button view reference</param>
        [Java.Interop.Export("lightSwitch_Click")]
        public async void lightSwitch_Click(View view)
        {
            bool success = false;
            if (view.Tag != null)
            {
                // Split View Tags
                string[] viewTagTokens = view.Tag.ToString().Split('|');
                if (viewTagTokens.Length == 2) // Check for exactly two tokens
                {
                    string id = viewTagTokens[0]; // Extract lightId
                    string action = viewTagTokens[1]; // Extract action

                    // Trigger Light On
                    success = await TriggerLight(id, action);
                }
            }
        }

        /// <summary>
        /// Trigger light
        /// </summary>
        /// <param name="id">Light Id</param>
        /// <param name="action">Trigger action</param>
        /// <returns>Success?</returns>
        private async Task<bool> TriggerLight(string id, string action)
        {
            if (isHomeNetworkConnected)
            {
                return await new LightController().TriggerLight(id, action);
            }
            else
            {
                Toast.MakeText(ApplicationContext, "Home network not connected.", ToastLength.Long).Show();
                return false;
            }
        }
    }
}