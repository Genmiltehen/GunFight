using GameEngineLib.Defaults;
using GameEngineLib.Defaults.Render;
using GameEngineLib.Impl;
using GameEngineLib.Impl.SceneImpl;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsUI
{
    public class TestScene : Scene
    {
        public TestScene(IAssetLoader assetloader) : base(assetloader)
        {

        }

        public override void Load()
        {
            var entity = CreateEntity("player");
            entity.AddComponent<TransformComp>().Init(new Vector3(100, 100, 0), 0.5f, new Vector2(0.2f, 0.2f));
            entity.AddComponent<SpriteComp>().Init(AssetsLoader.LoadTexture("Test/test.jpg"));
        }
    }
}
