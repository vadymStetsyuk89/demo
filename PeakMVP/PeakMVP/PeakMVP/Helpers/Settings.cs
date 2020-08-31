using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace PeakMVP.Helpers {
    public class Settings {

        private static ISettings AppSettings => CrossSettings.Current;

        private const string SETTINGS_KEY = "settings_key";
        private const string USER_PROFILE = "user_profile";
        private static readonly string SettingsDefault = string.Empty;

        public static string UserProfile {
            get => AppSettings.GetValueOrDefault(USER_PROFILE, null);
            set => AppSettings.AddOrUpdateValue(USER_PROFILE, value);
        }

        public static string GeneralSettings {
            get { return AppSettings.GetValueOrDefault(SETTINGS_KEY, SettingsDefault); }
            set { AppSettings.AddOrUpdateValue(SETTINGS_KEY, value); }
        }
    }
}
