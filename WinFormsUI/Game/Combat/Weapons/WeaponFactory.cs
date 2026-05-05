using WinFormsUI.Game.Config;

namespace WinFormsUI.Game.Combat.Weapons
{
    public class WeaponFactory
    {
        private readonly ConfigLoader<WeaponConfig> configLoader;

        private static readonly Lazy<WeaponFactory> _instance = new(() => new WeaponFactory());
        public static WeaponFactory Instance => _instance.Value;
        private WeaponFactory()
        {
            configLoader = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Config", "weapons.json"));
        }

        public bool CreateWeapon(string id, out WeaponItem item)
        {
            item = null!;
            bool res = configLoader.TryGetConfig(id, out var wConfig);
            if (res) item = new(wConfig);
            return res;
        }
    }
}
