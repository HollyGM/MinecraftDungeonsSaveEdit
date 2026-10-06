using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
#nullable enable

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class Currency
    {
        [JsonPropertyName("count")]
        public ulong Count { get; set; } = 0;
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>Keeps any field not mapped above so it is written back unchanged.</summary>
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }
    }
}
