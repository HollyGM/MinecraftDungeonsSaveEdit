using MCDSaveEdit.Save.Models.Enums;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
#nullable enable

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class Difficulties
    {
        [JsonPropertyName("announced")]
        public DifficultyEnum? Announced { get; set; } = null;

        [JsonPropertyName("selected")]
        public DifficultyEnum? Selected { get; set; } = null;

        [JsonPropertyName("unlocked")]
        public DifficultyEnum Unlocked { get; set; } = DifficultyEnum.Difficulty_1;

        /// <summary>Keeps any field not mapped above so it is written back unchanged.</summary>
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }
    }
}
