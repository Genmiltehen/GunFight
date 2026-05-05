using Box2D.NET;
using OpenTK.Mathematics;
using XEngine.Core.Base;
using XEngine.Core.Utils.Maths;

namespace WinFormsUI.Game.Player.Components
{
    public class GPlayerMovement : GameComponent
    {
        public int GroundContacts { get; set; } = 0;
        public bool IsOnGround => GroundContacts > 0;

        public B2BodyId BodyId { get; private set; }

        private GPlayer _player = null!;

        public GPlayerMovement Init(B2BodyId bodyId)
        {
            _player = Owner.Get<GPlayer>()!;
            BodyId = bodyId;
            return this;
        }

        public void ApplyImpulse(Vector2 impulse)
        {
            B2Bodies.b2Body_ApplyLinearImpulseToCenter(BodyId, new(impulse.X, impulse.Y), true);
        }

        public Vector2 GetVelocity()
        {
            var vel = B2Bodies.b2Body_GetLinearVelocity(BodyId);
            return new Vector2(vel.X, vel.Y);
        }

        public void MovePlayer(float direction, float dt, float speedscale = 1.0f)
        {
            var vel = _player.Movement.GetVelocity();
            var acc = _player.Stats.Acceleration * dt;
            var tspeed = _player.Stats.TopSpeed * direction;
            var delta = MathUtils.MoveToward(vel.X, tspeed, acc) - vel.X;
            ApplyImpulse(new(delta * speedscale, 0));
            if (direction != 0) _player.IsRightFacing = _player.Model.SetFacingDiecration(new(direction, 0));
        }
    }
}
