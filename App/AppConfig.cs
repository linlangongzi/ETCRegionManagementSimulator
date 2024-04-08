using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace ETCRegionManagementSimulator
{
    public class AppConfig
    {
        private const string SettingsFileName = "AppSettings.json";

        private static ApplicationDataContainer LocalSettings => ApplicationData.Current.LocalSettings;

        public static void SaveSetting(string key, object value)
        {
            // TODO: Add more Settings
            LocalSettings.Values[key] = value;
        }

        public static T GetSetting<T>(string key, T defaultValue = default(T))
        {
            if (LocalSettings.Values.ContainsKey(key))
            {
                return (T)LocalSettings.Values[key];
            }
            return defaultValue;
        }

        public static async Task<T> LoadSettingsAsync<T>(string fileName)
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{fileName}"));
                //using (Stream stream = await file.OpenStreamForReadAsync())
                //{
                //    return await JsonSerializer.DeserializeAsync<T>(stream);
                //}
                return default(T);
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Error loading settings: {ex.Message}");
                return default(T);
            }
        }
    }
}
