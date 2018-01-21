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

namespace smarthome
{
    public class LightsListAdapter : BaseAdapter<Light>
    {
        Activity context = null;
        IList<Light> lights = new List<Light>();

        public LightsListAdapter(Activity context, IList<Light> lights) : base()
        {
            this.context = context;
            this.lights = lights;
        }

        public override Light this[int position]
        {
            get { return lights[position]; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get { return lights.Count; }
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            // Get our object for position
            var item = lights[position];

            //Try to reuse convertView if it's not  null, otherwise inflate it from our item layout
            // gives us some performance gains by not always inflating a new view
            // will sound familiar to MonoTouch developers with UITableViewCell.DequeueReusableCell()

            //			var view = (convertView ?? 
            //					context.LayoutInflater.Inflate(
            //					Resource.Layout.TaskListItem, 
            //					parent, 
            //					false)) as LinearLayout;
            //			// Find references to each subview in the list item's view
            //			var txtName = view.FindViewById<TextView>(Resource.Id.NameText);
            //			var txtDescription = view.FindViewById<TextView>(Resource.Id.NotesText);
            //			//Assign item's values to the various subviews
            //			txtName.SetText (item.Name, TextView.BufferType.Normal);
            //			txtDescription.SetText (item.Notes, TextView.BufferType.Normal);

            // TODO: use this code to populate the row, and remove the above view
            // use convertview to re-use or create new


            var view = (convertView ??
                                context.LayoutInflater.Inflate(
                                Resource.Layout.BtnListVwLayout,
                                parent,
                                false));

            view.FindViewById<TextView>(Resource.Id.name).Text = item.name;
            view.FindViewById<Button>(Resource.Id.on).Tag = $"{item.lightId}|on";
            view.FindViewById<Button>(Resource.Id.off).Tag = $"{item.lightId}|off";

            //Finally return the view
            return view;
        }
    }
}