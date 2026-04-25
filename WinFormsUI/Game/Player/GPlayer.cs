using Box2D.NET;
using System.Diagnostics.CodeAnalysis;
using WinFormsUI.Game.Box2D;
using WinFormsUI.Game.Player.PlayerStates;
using WinFormsUI.Game.Player.Stats;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Common;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Input;
using XEngine.Core.Scenery;
using XEngine.Core.Utils;

namespace WinFormsUI.Game.Player
{
    public class GPlayer : GameComponent
    {
        public string Name = "";
        public bool IsRightFacing = true;

        public Entity BodyEntity { get; private set; } = null!;
        public Entity HeadEntity { get; private set; } = null!;
        public Entity WeaponEntity { get; private set; } = null!;

        public readonly GameTimer JumpTimer = new(0.3f);
        public readonly GameTimer ShootTimer = new(0.5f);
        public PlayerEffectList Effects = null!;

        private IPlayerState? _state;

        public GPlayer Init(GScene scene, IPlayerStats stats)
        {
            BodyEntity = Owner;
            HeadEntity = Owner.GetChild(1)!;
            WeaponEntity = Owner.GetChild(0)!;

            Effects = new(stats);
            _state = new IdleState();

            scene.RegisterTimer(JumpTimer);
            scene.RegisterTimer(ShootTimer);
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

        public GPlayer SetFacing(B2Vec2 dir)
        {
            float angle = B2MathFunction.b2Atan2(dir.Y, dir.X);
            HeadEntity.Transform.Rotation = angle;
            WeaponEntity.Transform.Rotation = angle;

            if (IsRightFacing != dir.X > 0)
            {
                IsRightFacing = dir.X > 0;
                float flip = IsRightFacing ? 1 : -1;
                BodyEntity.Get<GSprite>()!.SetSize(new(flip, 1));
                HeadEntity.Get<GSprite>()!.SetSize(new(1, flip));
                WeaponEntity.Get<GSprite>()!.SetSize(new(1, flip));
            }
            return this;
        }

        public GPlayer SetName(string name)
        {
            Name = name;
            return this;
        }

        public IPlayerStats Stats => Effects.GetStats();

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
