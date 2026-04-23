using Box2D.NET;
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

        public GBox2DWorld Init(int pixelPerMetre)
        {
            PixelPerMetre = pixelPerMetre;
            B2WorldDef worldDef = B2Types.b2DefaultWorldDef();
            worldDef.gravity = new B2Vec2(0, -18);

            Id = B2Worlds.b2CreateWorld(worldDef);
            return this;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            B2Worlds.b2DestroyWorld(Id);
        }
    }
}
