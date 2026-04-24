using Box2D.NET;
using WinFormsUI.Game.Box2D;
using WinFormsUI.Game.Player.PlayerStates;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Common;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Input;
using XEngine.Core.Scenery;

namespace WinFormsUI.Game.Player
{
    public class GPlayer : GameComponent
    {
        // TODO: Вынос в компонент PlayerStats
        public float TopSpeed = 5;
        public float Acceleration = 30;
        public float JumpPower = 30;

        public string Name = "";

        public Entity BodyEntity;
        public Entity HeadEntity;
        public Entity WeaponEntity;

        private IPlayerState? _state;
        private bool _isRight = true;

        public GPlayer Init(Entity body, Entity head, Entity weapon)
        {
            BodyEntity = body;
            HeadEntity = head;
            WeaponEntity = weapon;

            _state = new IdleState();
            return this;
        }

        public void ProcessInput(GScene scene, float dt)
        {
            _state?.ProcessInput(this, scene, dt);
        }

        public float HorizontalInput(IInputService input)
        {
            return input.GetAxis($"Horizontal{Name}");
        }

        public void SetFacing(B2Vec2 dir)
        {
            float angle = B2MathFunction.b2Atan2(dir.Y, dir.X);
            HeadEntity.Get<GTransform>()!.Rotation = angle;
            WeaponEntity.Get<GTransform>()!.Rotation = angle;

            if (_isRight != dir.X > 0)
            {
                _isRight = dir.X > 0;
                float flip = _isRight ? 1 : -1;
                BodyEntity.Get<GSprite>()!.SetScale(new(flip, 1));
                HeadEntity.Get<GSprite>()!.SetScale(new(1, flip));
                WeaponEntity.Get<GSprite>()!.SetScale(new(1, flip));
            }
        }

        public void SwitchTo<T>() where T : IPlayerState, new()
        {
            _state?.Exit(this);
            _state = new T();
            _state.Enter(this);
        }

        public bool IsOnGround()
        {
            var bodyId = BodyEntity.Get<GBox2DBody>()!.Id;
            var userData = B2Bodies.b2Body_GetUserData(bodyId).GetRef<PlayerUserData>();
            if (userData == null) return false;
            return userData.GroundCollisions > 0;
        }

        public GPlayer SetCharacterTeaxtures(IAssetLoader assets, string name)
        {
            if (BodyEntity.TryGet<GSprite>(out var bodySprite))
                bodySprite.SetTexture(assets.LoadTexture($"Characters/{name}/Body.png"), true);
            if (HeadEntity.TryGet<GSprite>(out var headSprite))
                headSprite.SetTexture(assets.LoadTexture($"Characters/{name}/Head.png"), true);
            return this;
        }

        public GPlayer SetWeaponTexture(IAssetLoader assets, string name)
        {
            if (WeaponEntity.TryGet<GSprite>(out var weaponSprite))
                weaponSprite.SetTexture(assets.LoadTexture($"Weapons/{name}.png"), true);
            return this;
        }
    }
}
