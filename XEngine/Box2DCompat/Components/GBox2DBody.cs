using Box2D.NET;
using XEngine.Core.Base;
using XEngine.Core.Common.Transform;
using XEngine.Core.Scenery;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace XEngine.Core.Box2DCompat.Components
{
    public sealed class GBox2DBody : GameComponent, IDisposable
    {
        private bool _disposed = false;
        private bool _isCollisionCallbackEnabled = false;
        public B2BodyId Id;

        public Action<ContactWrapper>? OnCollisionEnter = null;
        public Action<ContactWrapper>? OnCollisionExit = null;

        #region Building
        public struct GBox2DBodyBuilder
        {
            private B2BodyDef _bodyDef = B2Types.b2DefaultBodyDef();
            private readonly GBox2DBody _targetBody;

            public GBox2DBodyBuilder(GBox2DBody body, GTransform tr)
            {
                _targetBody = body;
                _bodyDef.position = new B2Vec2(tr.Position.X, tr.Position.Y);
                _bodyDef.rotation = B2MathFunction.b2MakeRot(tr.Rotation);
            }

            public GBox2DBodyBuilder SetType(B2BodyType type)
            {
                _bodyDef.type = type;
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

            public GBox2DBodyBuilder SetBulletFlag(bool value)
            {
                _bodyDef.isBullet = value;
                return this;
            }

            public readonly GBox2DBody Build(B2WorldId worldId)
            {
                _targetBody.Id = B2Bodies.b2CreateBody(worldId, _bodyDef);
                return _targetBody;
            }
        }

        public GBox2DBodyBuilder Init(GTransform tr)
        {
            return new GBox2DBodyBuilder(this, tr);
        }

        public GBox2DBody EnableCollisionCallback()
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
        #endregion

        public void CollisionEnter(GScene scene, ContactWrapper contactEvent)
        {
            scene.Schedule(() => OnCollisionEnter?.Invoke(contactEvent));
        }
        public void CollisionExit(GScene scene, ContactWrapper contactEvent)
        {
            scene.Schedule(() => OnCollisionExit?.Invoke(contactEvent));
        }

        public float GravityScale
        {
            get => B2Bodies.b2Body_GetGravityScale(Id);
            set => B2Bodies.b2Body_SetGravityScale(Id, value);
        }
        public float AngularVelocity
        {
            get => B2Bodies.b2Body_GetAngularVelocity(Id);
            set => B2Bodies.b2Body_SetAngularVelocity(Id, value);
        }
        public B2Vec2 LinearVelocity
        {
            get => B2Bodies.b2Body_GetLinearVelocity(Id);
            set => B2Bodies.b2Body_SetLinearVelocity(Id, value);
        }

        public void ApplyImpulse(B2Vec2 impulse) => B2Bodies.b2Body_ApplyLinearImpulseToCenter(Id, impulse, true);
        public void ApplyImpulse(float x = 0, float y = 0) => B2Bodies.b2Body_ApplyLinearImpulseToCenter(Id, new(x, y), true);

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            B2Bodies.b2DestroyBody(Id);
        }
    }
}
