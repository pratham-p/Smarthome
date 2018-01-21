
using Android.App;
using Android.Content;
using Android.Net;
using smarthome.DataService.Interfaces;

namespace smarthome.DataService.Services
{
    public class ConnectionService : IConnectionService
    {
        public static IConnectionService Instance { get; } = new ConnectionService();

        private readonly ConnectivityManager m_Manager;

        private ConnectionService()
        {
            m_Manager = (ConnectivityManager)Application.Context.GetSystemService(Context.ConnectivityService);
        }

        public bool IsConnected => m_Manager.ActiveNetworkInfo?.IsConnected ?? false;

        public NetworkInfo ConnectionInfo => m_Manager.ActiveNetworkInfo;

        public ConnectionType? Type
        {
            get
            {
                var activeNetwork = m_Manager.ActiveNetworkInfo;
                if (activeNetwork?.IsConnected ?? false)
                {
                    switch (activeNetwork.Type)
                    {
                        case ConnectivityType.Mobile:
                            return ConnectionType.Mobile;
                        case ConnectivityType.Wifi:
                            return ConnectionType.Wifi;
                        default:
                            return ConnectionType.Other;
                    }
                }
                return null;
            }
        }
    }
}