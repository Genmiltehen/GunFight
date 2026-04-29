using Box2D.NET;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
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
        public string CharacterSpriteName = "";
        public string HeadIdle => $"Characters/{CharacterSpriteName}/HeadIdle.png";
        public string HeadAiming => $"Characters/{CharacterSpriteName}/HeadAiming.png";
        public string HeadMove => $"Characters/{CharacterSpriteName}/HeadMove.png";
        public string BodyIdle => $"Characters/{CharacterSpriteName}/BodyIdle.png";
        public string BodyAiming => $"Characters/{CharacterSpriteName}/BodyAiming.png";
        public string BodyJump => $"Characters/{CharacterSpriteName}/BodyJump.png";
        public string BodyMoveL => $"Characters/{CharacterSpriteName}/BodyMoveL.png";
        public string BodyMoveM => $"Characters/{CharacterSpriteName}/BodyMoveM.png";
        public string BodyMoveR => $"Characters/{CharacterSpriteName}/BodyMoveR.png";

        public string Name = "NullPlayer";
        public bool IsRightFacing = true;

        public Entity BodyEntity { get; private set; } = null!;
        public Entity HeadEntity { get; private set; } = null!;
        public Entity WeaponEntity { get; private set; } = null!;

        public readonly GameTimer JumpTimer = new(0.3f);
        public readonly GameTimer ShootTimer = new(0.5f);
        public readonly GameTimer WalkTimer = new(0.15f);
        public PlayerEffectList Effects = null!;

        private IPlayerState? _state;
        public int GroundContacts = 0;

        public GPlayer Init(GScene scene, PlayerConfig config)
        {
            BodyEntity = Owner;
            HeadEntity = Owner.GetChild(1)!;
            WeaponEntity = Owner.GetChild(0)!;

            Effects = new(new PlayerStats(config));
            _state = new IdleState();

            scene.RegisterTimer(JumpTimer);
            scene.RegisterTimer(ShootTimer);
            scene.RegisterTimer(WalkTimer);
            return this;
        }

        public void ProcessInput(GScene scene, float dt)
        {
            _state?.ProcessInput(this, scene, dt);
        }

        public GPlayer SetName(string name)
        {
            Name = name;
            return this;
        }

        public IPlayerStats Stats => Effects.GetStats();

        public void SwitchTo<T>(GScene scene) where T : IPlayerState, new()
        {
            _state?.Exit(this, scene);
            _state = new T();
            _state.Enter(this, scene);
        }

        public GPlayer SetWeaponTexture(IAssetLoader assets, string name, float scale = 1)
        {
            if (WeaponEntity.TryGet<GSprite>(out var weaponSprite)) weaponSprite
                    .SetTexture(assets.LoadTexture($"Weapons/{name}.png"), true)
                    .SetSize(new(scale, scale));
            return this;
        }
    }
}
