using Box2D.NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public string Name = "";

        public Entity BodyEntity;
        public Entity HeadEntity;
        public Entity WeaponEntity;

        private IPlayerState? _state;

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
