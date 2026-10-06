using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;
#nullable enable

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class ThreatLevels
    {
        [JsonPropertyName("unlocked")]
        public string? Unlocked { get; set; } = null;

        /// <summary>Keeps any field not mapped above so it is written back unchanged.</summary>
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }
    }
}
