using Android.Net;

namespace smarthome.DataService.Interfaces
{
    public interface IConnectionService
    {
        bool IsConnected { get; }
        ConnectionType? Type { get; }
        NetworkInfo ConnectionInfo { get; }
    }

    public enum ConnectionType
    {
        Other,
        Wifi,
        Mobile
    }
}