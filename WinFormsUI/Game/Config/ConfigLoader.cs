using System.Diagnostics;
using System.Text.Json;

namespace WinFormsUI.Game.Config
{
    public class ConfigLoader<T> where T : class, IIdentifilable
    {
        private readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
        private readonly Dictionary<string, T> _database = [];

        public ConfigLoader(string _filePath)
        {
            if (!File.Exists(_filePath))
            {
                Debug.WriteLine($"[Warn]: Config file {_filePath} not. Config is empty.");
                return;
            }

            List<T>? configs = JsonSerializer.Deserialize<List<T>>(File.ReadAllText(_filePath), options);

            if (configs == null)
            {
                Debug.WriteLine($"[Warn]: Parser could not parse {_filePath}. Config empty.");
                return;
            }

            foreach (var config in configs)
                if (!_database.TryAdd(config.Id, config))
                    Debug.WriteLine($"[Warn]: Duplicate ID '{config.Id}' ignored.");
        }

        public bool TryGetConfig(string Id, out T res)
        {
            return _database.TryGetValue(Id, out res!);
        }

        public IEnumerable<T> GetAllConfigs()
        {
            return _database.Values;
        }
    }
}
