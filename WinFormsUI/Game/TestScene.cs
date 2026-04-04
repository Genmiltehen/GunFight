using OpenTK.Mathematics;
using XEngine.Core.Base;
using XEngine.Core.Defaults;
using XEngine.Core.Defaults.Sprite;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game
{
    public class TestScene(IAssetLoader assetloader) : Scene(assetloader)
    {
        public override void Load()
        {
            Entity entity;
            entity = CreateEntity();
            entity.AddComponent<PlayerTag>();
            entity.AddComponent<TransformComp>().Init(new Vector3(100, 100, 0), 0f, Vector2.One);
            entity.AddComponent<SpriteComp>().Init(AssetsLoader.LoadTexture("Test/test.png"));

            entity = CreateEntity();
            entity.AddComponent<PlayerTag>();
            entity.AddComponent<TransformComp>().Init(new Vector3(100, 100, -0.5f), 0f, Vector2.One);
            entity.AddComponent<SpriteComp>().Init(AssetsLoader.LoadTexture("Test/test.png"));

            AddSystem(new TestSystem());
        }
    }
}
