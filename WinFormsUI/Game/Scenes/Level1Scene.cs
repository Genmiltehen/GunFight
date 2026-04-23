using Box2D.NET;
using WinFormsUI.Game.Box2D;
using WinFormsUI.Game.Factories;
using WinFormsUI.Game.Player;
using XEngine.Core;
using XEngine.Core.Box2DCompat.Systems;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Scenes
{
    internal class Level1Scene : GScene
    {
        public Level1Scene(GameEngine _engine) : base(_engine)
        {
            LevelElementsFabctory.CreatePlatform(this, new(0, 0), new(20, 0.5f), 0);
            LevelElementsFabctory.CreatePlatform(this, new(-10, 5), new(5, 0.5f), 0);
        }

        public override void Load()
        {
            AddSystem(new PlayersControl(Input)); // 100
            AddSystem(new Box2DTransformSync(8)); // 400
            AddSystem(new Box2DOnGroundSystem()); // 550

            Camera.Transform.Position2D = new(0, 8);
            Camera.Zoom = -3;

            B2Vec2 charSize = new(1, 1.5f);

            var playerA = PlayerFactory.CreatePlayer(this, charSize);
            playerA.SetCharacterTeaxtures(Assets, "God");
            playerA.SetWeaponTexture(Assets, "Gun");
            playerA.Name = "A";
            playerA.SetFacing(new(-1, 0));

            var playerB = PlayerFactory.CreatePlayer(this, charSize);
            playerB.SetCharacterTeaxtures(Assets, "God");
            playerB.SetWeaponTexture(Assets, "Gun");
            playerB.Name = "B";
            playerB.SetFacing(new(-1, 0));
        }
    }
}
