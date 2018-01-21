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
using System.Threading.Tasks;
using System.Net;
using smarthome.DataService.Models;

namespace smarthome.DataService.Controllers
{
    public class LightController
    {
        private string _serverName { get; set; }
        private string _serverApiUrl
        {
            get
            {
                return $"http://{this._serverName}/api/v1/";
            }
        }

        public LightController(string serverName)
        {
            this._serverName = serverName;
        }

        public async Task<bool> TriggerLight(string lightId, string action)
        {
            //var root = new Lib.HttpHelper().DoPut<>
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new System.Uri(string.Format(this._serverApiUrl + "lights/{0}/{1}", lightId, action)));
            request.ContentType = "application/json";
            request.Method = "PUT";

            // Send the request to the server and wait for the response:
            using (WebResponse response = await request.GetResponseAsync())
            {
                return true;
            }
        }

        public List<Light> GetLights()
        {
            var response = new Lib.HttpHelper(this._serverApiUrl).DoGet<Response.LightsResponse>("lights");

            if (response != null)
            {
                List<Light> lights = response.lights.ToList();
                lights.Insert(0, new Light() { name = "All", lightId = "all" }); // Add "All" light object at the top.
                return lights;
            }
            else
                return null;
        }

        public bool PingTest()
        {
            var response = new Lib.HttpHelper(this._serverApiUrl).DoGet<string>("ping");

            if (response != null && response.Equals("OK", StringComparison.CurrentCultureIgnoreCase))
                return true;
            else
                return false;
        }
    }
}