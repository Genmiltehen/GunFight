using GameEngineLib.Defaults;
using GameEngineLib.Impl;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngineLib.Impl.RenderImpl
{
    public sealed class CameraComp : GameComponent
    {
        public RenderPipeline _renderer;

        public CameraComp Init(RenderPipeline renderer)
        {
            _renderer = renderer;
            return this;
        }

        public TransformComp Transform => Owner.Get<TransformComp>()!;

        public Matrix4 GetViewMatrix()
        {
            var worldMatrix = Transform.GetMatrix();
            return Matrix4.Invert(worldMatrix);
        }
    }
}
