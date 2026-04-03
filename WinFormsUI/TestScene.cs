using GameEngineLib.Defaults;
using GameEngineLib.Impl;
using GameEngineLib.Impl.SceneImpl;
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
            var entity = new Entity(
                new SpriteComp(AssetsLoader.LoadTexture("Test/test.png")),
                new TransformComp()
            );

            AddEntity(entity);


        }
    }
}
