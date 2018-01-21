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

namespace smarthome
{
    public class MyAppNotification
    {
        Context appContext = null;
        public MyAppNotification(Context context)
        {
            appContext = context;
        }

        public void SendNotification(string title, string text, int iconResource, int notificationId)
        {
            Notification.Builder builder = new Notification.Builder(appContext)
                                                            .SetContentTitle(title)
                                                            .SetContentText(text)
                                                            .SetSmallIcon(iconResource)
                                                            .SetAutoCancel(true);

            // Build the notification:
            Notification notification = builder.Build();

            // Get the notification manager:
            NotificationManager notificationManager =
                appContext.GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            notificationManager.Notify(notificationId, notification);
        }

        public void RemoveNotification(int notificationId)
        {
            // Get the notification manager:
            NotificationManager notificationManager =
                appContext.GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            notificationManager.Cancel(notificationId);
        }
    }
}