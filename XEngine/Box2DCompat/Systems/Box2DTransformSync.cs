using Box2D.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Common;
using XEngine.Core.Scenery;

namespace XEngine.Core.Box2DCompat.Systems
{
    public class Box2DTransformSync : IGameSystem
    {
        public int Priority => 400;
        public bool IsEnabled { get; set; } = true;
        private int _subSteps = 1;
        
        public Box2DTransformSync(int SubSteps) {
            _subSteps = SubSteps;
        }


        public void Update(GScene _scene, float _dt)
        {
            var pairs = _scene.Query<GTransform, GBox2DBody>();

            foreach (var (_, tr, b2b) in pairs) if (!tr.IsSynced) b2b.SyncToTransform(tr);
            B2Worlds.b2World_Step(_scene.World.Id, _dt, _subSteps);
            foreach (var (_, tr, b2b) in pairs) tr.SyncToBody(b2b);
        }
    }
}
