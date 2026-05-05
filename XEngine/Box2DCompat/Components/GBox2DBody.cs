using Box2D.NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Base;
using XEngine.Core.Common;
using XEngine.Core.Scenery;

namespace XEngine.Core.Box2DCompat.Components
{
    public sealed class GBox2DBody : GameComponent, System.IDisposable
    {
        private bool _disposed = false;
        private bool _isCollisionCallbackEnabled = false;
        public B2BodyId Id;

        public Action<ContactWrapper>? OnCollisionEnter = null;
        public Action<ContactWrapper>? OnCollisionExit = null;

        public class GBox2DBodyBuilder(GBox2DBody body)
        {
            private readonly GBox2DBody _targetBody = body;
            private B2BodyDef _bodyDef = B2Types.b2DefaultBodyDef();

            public GBox2DBodyBuilder SetType(B2BodyType type)
            {
                _bodyDef.type = type;
                return this;
            }

            public GBox2DBodyBuilder SyncToTransform(GTransform tr)
            {
                _bodyDef.position = new B2Vec2(tr.Position.X, tr.Position.Y);
                _bodyDef.rotation = B2MathFunction.b2MakeRot(tr.Rotation);
                return this;
            }

            public GBox2DBodyBuilder SetMotinLocks(B2MotionLocks motionLocks)
            {
                _bodyDef.motionLocks = motionLocks;
                return this;
            }

            public GBox2DBodyBuilder SetEnableSleep(bool value)
            {
                _bodyDef.enableSleep = value;
                return this;
            }

            public GBox2DBody Build(B2WorldId worldId)
            {
                _targetBody.Id = B2Bodies.b2CreateBody(worldId, _bodyDef);

                return _targetBody;
            }
        }

        public GBox2DBody EnableCollisionCallbacks()
        {
            if (!_isCollisionCallbackEnabled)
            {
                B2Bodies.b2Body_SetUserData(Id, B2UserData.Ref(new UserData(this)));
                _isCollisionCallbackEnabled = true;
            }
            return this;
        }

        public GBox2DBody DisableCollisionCallbacks()
        {
            if (_isCollisionCallbackEnabled)
            {
                B2Bodies.b2Body_SetUserData(Id, B2UserData.Empty);
                _isCollisionCallbackEnabled = false;
            }
            return this;
        }

        public GBox2DBodyBuilder Init()
        {
            return new GBox2DBodyBuilder(this);
        }

        public GBox2DBody AttacShapes(Action<B2BodyId> attachShapes)
        {
            attachShapes(Id);
            return this;
        }

        public void SyncToTransform(GTransform tr)
        {
            B2Vec2 pos = new(tr.Position.X, tr.Position.Y);
            B2Rot rot = B2MathFunction.b2MakeRot(tr.Rotation);
            B2Bodies.b2Body_SetTransform(Id, pos, rot);
        }

        public void CollisionEnter(GScene scene, ContactWrapper contactEvent)
        {
            scene.Schedule(() => OnCollisionEnter?.Invoke(contactEvent));
        }

        public void CollisionExit(GScene scene, ContactWrapper contactEvent)
        {
            scene.Schedule(() => OnCollisionExit?.Invoke(contactEvent));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            B2Bodies.b2DestroyBody(Id);
        }
    }
}
