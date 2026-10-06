using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MCDSaveEdit.Save.Models.Mapping;
#nullable enable

namespace MCDSaveEdit.Save.Models.Profiles
{
    public static class ProfileParser
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions();

        static ProfileParser()
        {
            _options.Converters.Add(new AttributeBasedConverterFactory());
            _options.Converters.Add(new GuidConverterFactory());
            _options.Converters.Add(new JsonStringEnumConverter());
            //_options.Converters.Add(new TextDoubleJsonConverter());
            _options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            _options.WriteIndented = true;
            _options.AllowTrailingCommas = true;
        }

        public static async ValueTask<ProfileSaveFile?> Read(Stream stream)
        {
            using MemoryStream raw = new MemoryStream();
            await stream.CopyToAsync(raw);
            using MemoryStream sanitized = sanitize(raw.GetBuffer(), (int)raw.Length);

            ProfileSaveFile? obj = await JsonSerializer.DeserializeAsync<ProfileSaveFile>(sanitized, _options);
            //TODO: handle migration of Tower data from 1.15.1 to 1.16.2
            //if(obj?.MissionStatesMap?.TowerMissionContainer?.OldTowerMissionStateData != null && obj!.MissionStatesMap!.TowerMissionContainer!.OldTowerMissionStateData.Count > 0)
            //{
            //    var json = JsonSerializer.Serialize(obj!.MissionStatesMap!.TowerMissionContainer!.OldTowerMissionStateData);
            //    var oldTowerMissionStateData = JsonSerializer.Deserialize<TowerMissionState>(json);
            //    obj.MissionStatesMap.TowerMissionContainer = new TowerMissionContainer() { TowerMissionStates = new[] { oldTowerMissionStateData } };
            //    obj!.MissionStatesMap!.TowerMissionContainer!.OldTowerMissionStateData = null;
            //}
            return obj;
        }

        /// <summary>
        /// Strips ASCII control bytes and keeps only the first complete JSON object.
        /// PS4 and Switch files have a bunch of extra duplicate data at the end
        /// which makes them invalid json files.
        /// </summary>
        /// <remarks>
        /// Works on raw UTF-8 bytes: only bytes below 0x20 and 0x7F are dropped, so the
        /// continuation bytes of multi-byte characters are preserved. Braces inside
        /// string values are not counted.
        /// </remarks>
        internal static MemoryStream sanitize(byte[] buffer, int length)
        {
            var sanitized = new MemoryStream(length);
            int bracketCount = 0;
            bool inString = false;
            bool escaped = false;
            for (int i = 0; i < length; i++)
            {
                byte b = buffer[i];
                if (b < 0x20 || b == 0x7F)
                {
                    continue;
                }

                sanitized.WriteByte(b);

                if (inString)
                {
                    if (escaped)
                    {
                        escaped = false;
                    }
                    else if (b == (byte)'\\')
                    {
                        escaped = true;
                    }
                    else if (b == (byte)'"')
                    {
                        inString = false;
                    }
                }
                else if (b == (byte)'"')
                {
                    inString = true;
                }
                else if (b == (byte)'{')
                {
                    bracketCount++;
                }
                else if (b == (byte)'}')
                {
                    bracketCount--;
                    if (bracketCount == 0)
                    {
                        // This is the end of the first full JSON object
                        break;
                    }
                }
            }

            sanitized.Seek(0, SeekOrigin.Begin);
            return sanitized;
        }

        public static async ValueTask<Stream> Write(ProfileSaveFile settings)
        {
            Stream stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, settings, _options);
            return stream;
        }
    }
}
