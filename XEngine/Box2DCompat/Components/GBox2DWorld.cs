using Box2D.NET;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Base;

namespace XEngine.Core.Box2DCompat.Components
{
    public sealed class GBox2DWorld : GameComponent, IDisposable
    {
        public B2WorldId Id;
        public int PixelPerMetre = 1;
        private bool _disposed = false;
        private Matrix4 _ppmScale = Matrix4.Identity;

        public GBox2DWorld Init(int pixelPerMetre, B2Vec2 gravity)
        {
            PixelPerMetre = pixelPerMetre;
            _ppmScale = Matrix4.CreateScale(1.0f / pixelPerMetre, 1.0f / pixelPerMetre, 1);
            B2WorldDef worldDef = B2Types.b2DefaultWorldDef();
            worldDef.gravity = gravity;

            Id = B2Worlds.b2CreateWorld(worldDef);
            return this;
        }

        public Matrix4 PPMScale => _ppmScale;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            B2Worlds.b2DestroyWorld(Id);
        }
    }
}
