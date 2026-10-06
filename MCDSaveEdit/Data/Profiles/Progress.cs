using MCDSaveEdit.Save.Models.Enums;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class Progress
    {
        [JsonPropertyName("completedDifficulty")]
        public string CompletedDifficulty { get; set; }

        [JsonPropertyName("completedEndlessStruggle")]
        public int CompletedEndlessStruggle { get; set; }

        [JsonPropertyName("completedThreatLevel")]
        public string CompletedThreatLevel { get; set; }

        /// <summary>Keeps any field not mapped above so it is written back unchanged.</summary>
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }
    }
}
