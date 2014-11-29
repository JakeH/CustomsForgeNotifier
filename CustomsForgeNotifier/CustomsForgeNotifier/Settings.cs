using Nini.Config;
using System;

namespace CustomsForgeNotifier
{
    /// <summary>
    /// Application setting
    /// </summary>
    public static class Settings
    {
        private static readonly IConfigSource ConfigSource;
        private static readonly IConfig AppConfig;

        /// <summary>
        /// Pushbullet settings
        /// </summary>
        public static readonly PushbulletSettings Pushbullet;

        /// <summary>
        /// Notifier service to be used
        /// </summary>
        public static readonly string Notifier;

        /// <summary>
        /// Maximum number of entries to be returned when querying the Customs Forge service
        /// </summary>
        public static readonly int AbsoluteRetrievalLimit;

        /// <summary>
        /// The update time from the last known entry
        /// </summary>
        public static DateTime LastEntryUpdated
        {
            get
            {
                return new DateTime(AppConfig.GetLong("LastEntryUpdated", 0), DateTimeKind.Utc);
            }
            set
            {
                AppConfig.Set("LastEntryUpdated", value.Ticks);
            }
        }

        static Settings()
        {
            ConfigSource = new IniConfigSource("settings.ini");
            ConfigSource.AutoSave = true;

            AppConfig = ConfigSource.Configs["App"];

            var pushConfig = ConfigSource.Configs["Pushbullet"];

            Pushbullet = new PushbulletSettings
            {
                APIUri = pushConfig.GetString("APIUri"),
                APIToken = pushConfig.GetString("APIToken"),
                DeviceIden = pushConfig.GetString("DeviceIden")
            };

            Notifier = AppConfig.GetString("Notifier");
            AbsoluteRetrievalLimit = AppConfig.GetInt("AbsoluteRetrievalLimit", 25);
        }
    }

    public struct PushbulletSettings
    {
        public string APIUri;
        public string APIToken;
        public string DeviceIden;
    }
}