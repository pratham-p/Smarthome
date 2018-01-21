namespace smarthome.DataService.Interfaces
{
    public interface ISharedPreferenceService
    {
        T GetSetting<T>(string key);
        bool SaveSetting<T>(string key, T value);
        bool RemoveSetting(string key);
    }
}