using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using MCDSaveEdit.Save.Models.Enums;

namespace MCDSaveEdit.Save.Models.Profiles
{
    public partial class Armorproperty
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("rarity")]
        public Rarity Rarity { get; set; }

        /// <summary>Keeps any field not mapped above so it is written back unchanged.</summary>
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }
    }

    public partial class Armorproperty : ICloneable
    {
        public object Clone()
        {
            return Copy();
        }

        public Armorproperty Copy()
        {
            var copy = new Armorproperty();
            copy.Id = this.Id;
            copy.Rarity = this.Rarity;
            return copy;
        }
    }
}
