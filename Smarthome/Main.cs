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
using smarthome.DataService.Models;
using smarthome.DataService.Controllers;
using Android.Content.PM;
using Android.Support.V4.Widget;
using System.Threading.Tasks;
using Android.Util;
using Android.Support.V7.App;
using smarthome.DataService.Lib;
using Android.Support.Design.Widget;
using smarthome.DataService.Services;
using smarthome.DataService.Interfaces;

namespace smarthome
{
    [Activity(Label = "@string/ApplicationName",
        ScreenOrientation = ScreenOrientation.Portrait,
        MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/SmartlightsTheme")]
    public class Main : AppCompatActivity
    {
        string TAG = "Smartlights";
        bool isHomeNetworkConnected = false;
        int wifiRed = Resource.Drawable.ic_wifi_red;
        int wifiGreen = Resource.Drawable.ic_wifi_green;
        LinearLayout rootView;
        ImageView wifiImage;
        SwipeRefreshLayout swipeRefreshLayout;
        string[] defaultMessage = new string[] { Constants.NoNetworkServerMessage, Constants.CheckSettingsMessage };
        ListView btnControlView;
        IList<Light> lights;

        Android.Support.V7.Widget.Toolbar toolbar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.MainScreen); // loads the HomeScreen.axml as this activity's view

            // Get reference to the controls in the layout
            btnControlView = FindViewById<ListView>(Resource.Id.BtnControlsList);
            wifiImage = FindViewById<ImageView>(Resource.Id.wifiImage);
            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.mainSwipeLayout);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            rootView = FindViewById<LinearLayout>(Resource.Id.rootView);

            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = Constants.AppTitle;
            toolbar.Elevation = 4;

            // set color scheme for swipe refresh
            swipeRefreshLayout.SetColorSchemeColors(Resource.Color.accent);

            // Attach Refresh event
            swipeRefreshLayout.Refresh += SwipeRefreshLayout_Refresh;

            // Check home network and create/update ui
            if (IsHomeNetworkConnected() && IsServerOnline())
            {
                isHomeNetworkConnected = true;
                SetWifiImage(wifiGreen);
            }

            //Bind ListView Control
            btnControlView.Adapter = GetListAdapter();
        }

        /// <summary>
        /// Options Menu OnCreate event handler
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu, menu);
            var settingMenuItem = menu.FindItem(Resource.Id.mnuSettings);
            return true;
        }

        /// <summary>
        /// Options Menu selected event handler
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.mnuSettings:
                    OpenSettings();
                    return true;
                case Resource.Id.mnuRefresh:
                    //Snackbar snackbar = Snackbar.Make(FindViewById(Resource.Id.mnuSettings), "Refreshing...", Snackbar.LengthLong);
                    //snackbar.Show();
                    Toast.MakeText(ApplicationContext, "Refreshing...", ToastLength.Short).Show();
                    Refresh();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
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



        #region Private methods

        /// <summary>
        /// Get List View Adapter
        /// </summary>
        /// <returns></returns>
        private BaseAdapter GetListAdapter()
        {
            // Check home network and create/update ui
            if (isHomeNetworkConnected)
            {
                var lights = GetLights();
                if (lights != null)
                    return new LightsListAdapter(this, lights);
                else
                    return new EmptyListAdapter(this, defaultMessage);
            }
            else
            {
                return new EmptyListAdapter(this, defaultMessage);
            }
        }

        /// <summary>
        /// Fetch Lights data and fill adapter
        /// </summary>
        private IList<Light> GetLights()
        {
            // Fetch List of Lights using api
            lights = new LightController(GetServerName()).GetLights();
            return lights ?? null;
        }

        /// <summary>
        /// Refresh Screen
        /// </summary>
        private async void Refresh()
        {
            // Asynchronous Task - Async-Await method
            try
            {
                await Task.Run(() =>
                {
                    // Check home network connection
                    isHomeNetworkConnected = (IsHomeNetworkConnected() && IsServerOnline());
                    System.Threading.Thread.Sleep(3000);
                });

                // Run on UI thread
                RunOnUiThread(() =>
                {
                    // Update ui
                    if (isHomeNetworkConnected)
                        SetWifiImage(wifiGreen);
                    else
                        SetWifiImage(wifiRed);

                    swipeRefreshLayout.Refreshing = false;

                    //Bind ListView Control
                    btnControlView.Adapter = GetListAdapter();
                });
            }
            catch (Exception ex)
            {
                Log.Error($"{TAG}::{ex.GetType().Name}", $"{ex.Message}::{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Open Settings Dialog
        /// </summary>
        private void OpenSettings()
        {
            ISharedPreferenceService prefService = new SharedPreferenceService(Constants.PrefFileName);
            //set alert for executing the task
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);

            View settingsLayout = LayoutInflater.Inflate(Resource.Layout.Settings, null);
            alert.SetView(settingsLayout);
            alert.SetTitle("Settings");

            EditText network = settingsLayout.FindViewById<EditText>(Resource.Id.networkName);
            EditText server = settingsLayout.FindViewById<EditText>(Resource.Id.serverName);
            network.Text = prefService.GetSetting<string>(Constants.PrefHomeNetworkKey);
            server.Text = prefService.GetSetting<string>(Constants.PrefServerNameKey);

            /* Commented (For Ref)
            // Set an EditText view to get user input   
            EditText network = new EditText(this);
            network.Hint = "Home network name";
            network.SetPadding(GetDpFromPx(5), GetDpFromPx(5), GetDpFromPx(5),0);
            network.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            alert.SetView(network);
            */

            alert.SetPositiveButton("Save", (senderAlert, args) =>
            {
                //change value write your own set of instructions
                //you can also create an event for the same in xamarin
                //instead of writing things here

                string networkNameValue = network.Text;
                string serverNameValue = server.Text;
                Log.Debug(TAG, "Server name value : " + serverNameValue);
                Log.Debug(TAG, "Network name value : " + networkNameValue);

                // Save Settings
                prefService.SaveSetting<string>(Constants.PrefServerNameKey, serverNameValue);
                prefService.SaveSetting<string>(Constants.PrefHomeNetworkKey, networkNameValue);
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                //perform your own task for this conditional button click
            });
            //run the alert in UI thread to display in the screen
            RunOnUiThread(() =>
            {
                alert.Create().Show();
                //alert.Show();
            });
        }

        //private void BtnControlView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        //{
        //    var listView = sender as ListView;
        //    var t = lights[e.Position];
        //    Toast.MakeText(this, t.name, ToastLength.Short).Show();
        //}

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
        private void SwipeRefreshLayout_Refresh(object sender, EventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// Check home network connection
        /// </summary>
        /// <returns>IsConnected?</returns>
        private bool IsHomeNetworkConnected()
        {
            var homeNetworkName = GetHomeNetworkName();
            if (!string.IsNullOrWhiteSpace(homeNetworkName))
                return new ConnectionManager().IsOnline(ApplicationContext, homeNetworkName);
            else
                return false;
        }

        /// <summary>
        /// Check if the server/app is up and running
        /// </summary>
        /// <returns>IsOnline?</returns>
        private bool IsServerOnline()
        {
            var serverName = GetServerName();

            if (!string.IsNullOrWhiteSpace(serverName))
                return new LightController(serverName).PingTest();
            else
                return false;
        }

        /// <summary>
        /// Get Server Name
        /// </summary>
        /// <returns></returns>
        private string GetServerName()
        {
            var serverName = (new SharedPreferenceService(Constants.PrefFileName)).GetSetting<string>(Constants.PrefServerNameKey);

            if (!string.IsNullOrWhiteSpace(serverName))
                return serverName;
            else
            {
                Toast.MakeText(this, "Please set Server IP/Name.", ToastLength.Long).Show();
                return string.Empty;
            }
        }

        /// <summary>
        /// Get Home Network Name
        /// </summary>
        /// <returns></returns>
        private string GetHomeNetworkName()
        {
            var networkName = (new SharedPreferenceService(Constants.PrefFileName)).GetSetting<string>(Constants.PrefHomeNetworkKey);

            if (!string.IsNullOrWhiteSpace(networkName))
                return networkName;
            else
            {
                Toast.MakeText(this, "Please set Home network name.", ToastLength.Long).Show();
                return string.Empty;
            }
        }

        /// <summary>
        /// Set Wifi connection image on ui
        /// </summary>
        /// <param name="resourceId">Image resource id</param>
        private void SetWifiImage(int resourceId)
        {
            wifiImage.SetImageResource(resourceId);
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
                return await new LightController(GetServerName()).TriggerLight(id, action);
            }
            else
            {
                Toast.MakeText(ApplicationContext, "Home network not connected.", ToastLength.Long).Show();
                return false;
            }
        }
        #endregion
    }
}