using GameAnalyticsSDK.Net;
using System;
using System.Threading.Tasks;
#nullable enable

namespace MCDSaveEdit.Services
{
    public class GameAnalyticsRemoteConfig: Config
    {
        private readonly TimeSpan configDownloadTimeout = TimeSpan.FromSeconds(5);
        private readonly TimeSpan configPollInterval = TimeSpan.FromMilliseconds(100);

        public GameAnalyticsRemoteConfig() : base() { }

        public override async Task downloadAsync()
        {
            var timeoutTime = DateTime.Now + configDownloadTimeout;
            if (!GameAnalytics.IsRemoteConfigsReady())
            {
                Console.WriteLine("Waiting for configs...");
            }
            //Poll instead of spinning, which kept a core busy and flooded the console output
            while (!GameAnalytics.IsRemoteConfigsReady() && DateTime.Now < timeoutTime)
            {
                await Task.Delay(configPollInterval).ConfigureAwait(false);
            }
            if (GameAnalytics.IsRemoteConfigsReady())
            {
                stableReleaseVersionString = GameAnalytics.GetRemoteConfigsValueAsString("STABLE_VER", string.Empty);
                betaReleaseVersionString = GameAnalytics.GetRemoteConfigsValueAsString("BETA_VER", string.Empty);
                isConfigsReady = true;
            }
        }

    }
}
