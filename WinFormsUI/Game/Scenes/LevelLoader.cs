using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using WinFormsUI.Game.Scenes.LOCs;

namespace WinFormsUI.Game.Scenes
{
    public class LevelLoader
    {
        private static readonly JsonSerializerOptions options = new()
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver { Modifiers = { PolySupport } },
            PropertyNameCaseInsensitive = true
        };
        
        private static void PolySupport(JsonTypeInfo typeInfo)
        {
            if (typeInfo.Type == typeof(BaseLOC))
            {
                typeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                {
                    TypeDiscriminatorPropertyName = "$type",
                    IgnoreUnrecognizedTypeDiscriminators = true,
                    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                    DerivedTypes = {
                        new JsonDerivedType(typeof(TowerLOC), "tower"),
                        new JsonDerivedType(typeof(PlatformLOC), "platform"),
                        new JsonDerivedType(typeof(PlayerLOC), "player"),
                        new JsonDerivedType(typeof(LadderLOC), "ladder"),
                        new JsonDerivedType(typeof(EffectSpawnerLOC), "effectSpawner"),
                        new JsonDerivedType(typeof(WeaponSpawnerLOC), "weaponSpawner"),
                        new JsonDerivedType(typeof(BoxLOC), "box"),
                    }
                };
            }
        }

        public static IEnumerable<BaseLOC> Load(string path)
        {
            var _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Config", path);
            if (!File.Exists(_filePath))
            {
                Debug.WriteLine($"[Warn]: Config file {_filePath} not. Config is empty.");
                return [];
            }

            List<BaseLOC>? configs = JsonSerializer.Deserialize<List<BaseLOC>>(File.ReadAllText(_filePath), options);

            if (configs == null)
            {
                Debug.WriteLine($"[Warn]: Parser could not parse {_filePath}. Config empty.");
                return [];
            }

            return configs;
        }
    }
}
