using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class MerchantPricing
    {
        [JsonPropertyName("timesRestocked")]
        public long TimesRestocked { get; set; }

        /// <summary>Keeps any field not mapped above so it is written back unchanged.</summary>
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }
    }
}
