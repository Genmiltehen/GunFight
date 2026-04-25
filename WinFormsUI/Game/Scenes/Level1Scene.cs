using Box2D.NET;
using OpenTK.Mathematics;
using WinFormsUI.Game.Box2D;
using WinFormsUI.Game.Factories;
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
    internal class Level1Scene(GameEngine _engine) : GScene(_engine)
    {
        public override void Load()
        {
            AddSystem(new CameraSystem(10)); // 100
            AddSystem(new PlayersSystem(Input)); // 100
            AddSystem(new Box2DTransformSync(8)); // 400
            AddSystem(new Box2DOnGroundSystem()); // 550

            CreateBG("Environment\\Background");

            LevelElementsFabctory.CreatePlatform(this, new(0, -4, 0), new(20, 8), 0);
            LevelElementsFabctory.CreatePlatform(this, new(-10, -1, 0), new(6, 14), 0);

            Camera.Owner.Transform.Position2D = new(0, 8);

            var playerA = PlayerFactory.CreatePlayer(this, new(-10, 6), "A", new PlayerStats(3, 35, 15));
            //var speedup = new SpeedUpEffect(2);
            //playerA.Effects.Add(speedup);
            //playerA.Effects.Add(new JumpBoostEffect(2));
            //playerA.Effects.Remove(speedup);
            playerA.SetCharacterTeaxtures(Assets, "God");
            playerA.SetWeaponTexture(Assets, "Gun");

            var playerB = PlayerFactory.CreatePlayer(this, new(1, 0), "B", new PlayerStats(3, 35, 15));
            playerB.SetCharacterTeaxtures(Assets, "God");
            playerB.SetWeaponTexture(Assets, "Gun");
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
    }
}
