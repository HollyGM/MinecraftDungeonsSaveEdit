using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public class MerchantQuest
    {
        [JsonPropertyName("dynamicQuestState")]
        public object DynamicQuestState { get; set; }

        [JsonPropertyName("questState")]
        public object QuestState { get; set; }

        /// <summary>Keeps any field not mapped above so it is written back unchanged.</summary>
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }
    }
}
