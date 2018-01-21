using Android.App;
using Android.Content;
using smarthome.DataService.Interfaces;
using System;

namespace smarthome.DataService.Services
{
    public class SharedPreferenceService : ISharedPreferenceService
    {
        // Android ISharedPreferences
        private readonly Android.Content.ISharedPreferences sharedPrefs;

        public SharedPreferenceService(string fileName)
        {
            sharedPrefs = Application.Context.GetSharedPreferences(fileName, FileCreationMode.Private);
        }

        public bool SaveSetting<T>(string key, T value)
        {
            //store
            var prefEditor = sharedPrefs.Edit();

            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Int32:
                    // Handle int
                    prefEditor.PutInt(key, Convert.ToInt32(value));
                    break;
                case TypeCode.Boolean:
                    // Handle boolean
                    prefEditor.PutBoolean(key, Convert.ToBoolean(value));
                    break;
                case TypeCode.Single:
                    //Handle float
                    prefEditor.PutFloat(key, Convert.ToSingle(value));
                    break;
                case TypeCode.Int64:
                    //Handle long
                    prefEditor.PutLong(key, Convert.ToInt64(value));
                    break;
                case TypeCode.String:
                    //Handle string
                    prefEditor.PutString(key, Convert.ToString(value));
                    break;
            }
            return prefEditor.Commit();
        }

        public T GetSetting<T>(string key)
        {
            object value;
            //retreive 
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Int32:
                    // Handle int
                    value = sharedPrefs.GetInt(key, 0);
                    break;
                case TypeCode.Boolean:
                    // Handle boolean
                    value = sharedPrefs.GetBoolean(key, false);
                    break;
                case TypeCode.Single:
                    //Handle float
                    value = sharedPrefs.GetFloat(key, 0);
                    break;
                case TypeCode.Int64:
                    //Handle long
                    value = sharedPrefs.GetLong(key, 0);
                    break;
                case TypeCode.String:
                    //Handle string
                    value = sharedPrefs.GetString(key, null);
                    break;
                default:
                    value = null;
                    break;
            }

            return (T)value;
        }

        public bool RemoveSetting(string key)
        {
            var prefEditor = sharedPrefs.Edit();
            prefEditor.Remove(key);

            return prefEditor.Commit();
        }
    }
}