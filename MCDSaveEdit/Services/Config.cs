using MCDSaveEdit.Data;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
#nullable enable

namespace MCDSaveEdit.Services
{
    public class Config
    {
        public static Config instance = new GameAnalyticsRemoteConfig();

        public bool isConfigsReady { get; protected set; }

        public bool showInventoryIndexOrEquipmentSlot {
            get {
                return Constants.IS_DEBUG;
            }
        }

        public bool showIDsInSelectionWindow {
            get {
                return Constants.IS_DEBUG;
            }
        }

        public string? stableReleaseVersionString { get; protected set; }
        public string? betaReleaseVersionString { get; protected set; }

        public virtual async Task downloadAsync()
        {
            //Using GitHub
            try
            {
                var request = WebRequest.Create(Constants.LATEST_RELEASE_GITHUB_URL);
                using var response = await request.GetResponseAsync();
                stableReleaseVersionString = response.ResponseUri.Segments.Last();
                betaReleaseVersionString = string.Empty;
                isConfigsReady = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public virtual string newVersionDownloadURL()
        {
            return Constants.LATEST_RELEASE_GITHUB_URL;
        }

        public string versionLabel()
        {
            var stableVersion = parseVersion(stableReleaseVersionString);
            var betaVersion = parseVersion(betaReleaseVersionString);
            if (stableVersion != null && Constants.CURRENT_VERSION.Equals(stableVersion))
            {
                return R.STABLE_VERSION_LABEL;
            }
            if (betaVersion != null && Constants.CURRENT_VERSION.Equals(betaVersion))
            {
                return R.BETA_VERSION_LABEL;
            }
            if (betaVersion != null && Constants.CURRENT_VERSION > betaVersion)
            {
                return R.UNRELEASED_VERSION_LABEL;
            }
            return R.OLD_VERSION_LABEL;
        }

        public bool isNewBetaVersionAvailable()
        {
            var betaVersion = parseVersion(betaReleaseVersionString);
            return betaVersion != null && Constants.CURRENT_VERSION < betaVersion;
        }

        public bool isNewStableVersionAvailable()
        {
            var stableVersion = parseVersion(stableReleaseVersionString);
            return stableVersion != null && Constants.CURRENT_VERSION < stableVersion;
        }

        /// <summary>
        /// Accepts release tags like "1.5.8.0" or "v1.5.8.0" and returns null for anything else
        /// instead of throwing, e.g. when the remote value is malformed.
        /// </summary>
        public static Version? parseVersion(string? versionString)
        {
            if (string.IsNullOrWhiteSpace(versionString))
            {
                return null;
            }
            var trimmed = versionString!.Trim().TrimStart('v', 'V');
            return Version.TryParse(trimmed, out var version) ? version : null;
        }

    }
}
