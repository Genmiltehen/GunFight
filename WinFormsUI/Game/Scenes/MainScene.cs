using OpenTK.Mathematics;
using System.Diagnostics;
using WinFormsUI.Game.Combat.Projectiles;
using WinFormsUI.Game.Combat.Weapons;
using WinFormsUI.Game.Config;
using WinFormsUI.Game.Drop;
using WinFormsUI.Game.Player;
using WinFormsUI.Game.Player.Stats;
using WinFormsUI.Game.Player.Stats.Effects;
using WinFormsUI.Game.Scenes.PlayerSpawner;
using XEngine.Core;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat.Systems;
using XEngine.Core.Common.Health;
using XEngine.Core.Common.LifeTime;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Common.Trace;
using XEngine.Core.Graphics.OpenGL;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Scenes
{
    internal class MainScene(GameEngine _engine) : GScene(_engine)
    {
        private void SetupSystems()
        {
            AddSystem(new Box2DContactSystem()); // 50
            AddSystem(new CameraSystem(10)); // 100
            AddSystem(new PlayersSystem(Input)); // 100

            // 200
            AddSystem(new LifeTimerSystem());
            AddSystem(new TraceSystem());
            AddSystem(new HealthSystem());
            AddSystem(new EffectsSystem());
            AddSystem(new ProjectileSystem());

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
                e = SpawnEntity();

                e.Transform.Init(new(0, 0, -10 * j), 0);

                e.AddComponent<GSprite>()
                    .SetTexture(tex)
                    .SetSizingPolicy(SizingPolicy.Source)
                    .SetSize(Vector2.One * j);
            }
        }

        public override void Load()
        {
            AddSystem(new PlayerSpawnSystem("baldy", "baldy"));

            SetupSystems();
            CreateBG("Environment\\Background");
            Camera.Owner.Transform.Position2D = new(0, 8);

            foreach (var bloc in LevelLoader.Load("Levels/Level1.json")) bloc.Spawn(this);

            if (WeaponFactory.Instance.TryCreateWeapon("pistol", out var w))
            {
                w.Init(this);
                DropBuilder.Init(w).SetVelocity(new(0, 10), 2).Spawn(this, new Vector3(0, 5, 0));
            }
        }
    }
}
