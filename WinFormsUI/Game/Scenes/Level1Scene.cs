using Box2D.NET;
using OpenTK.Mathematics;
using WinFormsUI.Game.Box2D;
using WinFormsUI.Game.Combat.Projectiles;
using WinFormsUI.Game.Combat.Weapons;
using WinFormsUI.Game.Config;
using WinFormsUI.Game.Player;
using WinFormsUI.Game.Player.Stats;
using WinFormsUI.Game.Player.Stats.Effects;
using XEngine.Core;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat.Systems;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Graphics.OpenGL;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Scenes
{
    internal class Level1Scene : GScene
    {
        public readonly PlayerFactory playerFactory;
        public readonly ProjectileFactory projectileFactory;
        public readonly ConfigLoader<WeaponConfig> weaponLoader;

        private void SetupSystems()
        {
            AddSystem(new Box2DContactSystem()); // 50
            AddSystem(new CameraSystem(10)); // 100
            AddSystem(new PlayersSystem(Input)); // 100
            AddSystem(new Box2DTransformSync(12)); // 400
        }

        private void CreateBG(string folder)
        {
            Texture2D tex;
            Entity e;

            for (int i = 1; i <= 4; i++)
            {
                float j = 6 - i;
                string path = Path.Combine(folder, $"{i}.png");
                tex = Assets.LoadTexture(path);
                e = CreateEntity();
                e.Transform.Init(new(0, 0, -10 * j), 0);
                e.AddComponent<GSprite>()
                    .SetTexture(tex, true)
                    .SetSize(Vector2.One * j);
            }
        }

        public Level1Scene(GameEngine _engine) : base(_engine)
        {
            var configPath = Path.Combine(Assets.RootPath, "Config");
            playerFactory = new(new($"{configPath}\\players.json"));
            weaponLoader = new($"{configPath}\\weapons.json");
            projectileFactory = new(new($"{configPath}\\projectiles.json"));
        }

        public override void Load()
        {
            SetupSystems();
            CreateBG("Environment\\Background");

            LevelElementsFabctory.CreatePlatform(this, new(0, -4, 0), new(20, 8), 0);
            LevelElementsFabctory.CreatePlatform(this, new(-10, -1, 0), new(6, 14), 0);

            LevelElementsFabctory.CreateBox(this, new(-3, 2, 0), new(2, 2), 0);
            LevelElementsFabctory.CreateBox(this, new(-3, 3, 0), new(1, 1), 0);

            Camera.Owner.Transform.Position2D = new(0, 8);

            playerFactory.CreatePlayer(this, new(-10, 6), "A", "baldy");
            playerFactory.CreatePlayer(this, new(1, 0), "B", "baldy");
        }
    }
}
