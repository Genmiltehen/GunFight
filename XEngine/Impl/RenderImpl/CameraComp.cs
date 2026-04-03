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
        public int Priority { get; private set; }
        public RenderPipeline _renderer;

        public CameraComp Init(RenderPipeline renderer, int priority)
        {
            _renderer = renderer;
            Priority = priority;
            _renderer.RegisterCamera(this);
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
