using WinFormsUI.Game.Config;

namespace WinFormsUI.Game.Combat.Projectiles
{
    public class ProjectileConfigLoader
    {
        private readonly ConfigLoader<ProjectileConfig> configLoader;
        private static readonly Lazy<ProjectileConfigLoader> _instance = new(() => new ProjectileConfigLoader());
        public static ProjectileConfigLoader Instance => _instance.Value;
        private ProjectileConfigLoader()
        {
            configLoader = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Config", "projectiles.json"));
        }

        public bool TryGetConfig(string projectileId, out ProjectileConfig pConfig)
        {
            return configLoader.TryGetConfig(projectileId, out pConfig);
        }
    }
}
