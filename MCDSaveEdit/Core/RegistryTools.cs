using Microsoft.Win32;
#nullable enable

namespace MCDSaveEdit
{
    public class RegistryTools
    {
        // Open (or create) the key holding the application's settings. The caller disposes it.
        private static RegistryKey openAppKey(string app_name)
        {
            return Registry.CurrentUser.CreateSubKey(@"Software\" + app_name);
        }

        // Save a value.
        public static void SaveSetting(string app_name, string name, object value)
        {
            using RegistryKey sub_key = openAppKey(app_name);
            sub_key.SetValue(name, value);
        }

        // Get a value. Falls back to the default when the stored value has an unexpected type.
        public static T GetSetting<T>(string app_name, string name, T default_value)
        {
            using RegistryKey sub_key = openAppKey(app_name);
            return sub_key.GetValue(name, default_value) is T value ? value : default_value;
        }

        // Delete a value.
        public static void DeleteSetting(string app_name, string name)
        {
            using RegistryKey sub_key = openAppKey(app_name);
            sub_key.DeleteValue(name, throwOnMissingValue: false);
        }
    }
}
