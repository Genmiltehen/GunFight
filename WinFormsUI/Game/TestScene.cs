using OpenTK.Mathematics;
using XEngine.Core;
using XEngine.Core.Base;
using XEngine.Core.Common;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Graphics;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game
{
    public class TestScene(GameEngine _engine) : Scene(_engine)
    {
        public override void Load()
        {
            Entity entity;
            entity = CreateEntity();
            entity.AddComponent<PlayerTag>();
            entity.AddComponent<TransformComp>().Init(new Vector3(100, 100, 0), 0f, Vector2.One);
            entity.AddComponent<SpriteComp>().Init(Assets.LoadTexture("Test/test.png"));

            entity = CreateEntity();
            entity.AddComponent<PlayerTag>();
            entity.AddComponent<TransformComp>().Init(new Vector3(100, 100, -0.5f), 0f, Vector2.One);
            entity.AddComponent<SpriteComp>().Init(Assets.LoadTexture("Test/test.png"));

            AddSystem(new TestSystem(Input));
        }
    }
}
