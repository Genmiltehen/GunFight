using Box2D.NET;
using OpenTK.Mathematics;
using System.Diagnostics;
using WinFormsUI.Game.Combat.Weapons;
using XEngine.Core.Base;
using XEngine.Core.Common;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Utils.Maths;

namespace WinFormsUI.Game.Player.Components
{
    public class GPlayerModel : GameComponent
    {
        public const float depth = 0.3f;
        public static readonly B2Vec2 PLAYER_SIZE = new(1f, 2f);
        public Vector2 pivot = new(PLAYER_SIZE.X * depth, PLAYER_SIZE.Y * 0.8f);
        public Vector2 HeldWeaponMuzzleOffset { get; private set; } = Vector2.Zero;

        public Entity HeadEntity { get; private set; } = null!;
        public Entity WeaponEntity { get; private set; } = null!;

        private string Base => $"Characters/{CharacterSpriteName}";

        public string CharacterSpriteName { get; set; } = "";

        public GPlayerModel Init(PlayerConfig config)
        {
            HeadEntity = AddBodyPart(Owner);
            WeaponEntity = AddBodyPart(Owner);
            WeaponEntity.Transform.Layer = -0.01f;
            CharacterSpriteName = config.TextureName;
            return this;
        }

        public void SetIdling()
        {
            SetHeadTexture($"{Base}/HeadIdle.png");
            SetBodyTexture($"{Base}/BodyIdle.png");
        }

        public void SetFall()
        {
            SetHeadTexture($"{Base}/HeadIdle.png");
            SetBodyTexture($"{Base}/BodyJump.png");
        }

        public void SetMoving(int idx)
        {
            SetHeadTexture($"{Base}/HeadMove.png");
            SetBodyTexture(idx switch
            {
                0 => $"{Base}/BodyMoveL.png",
                1 => $"{Base}/BodyMoveM.png",
                2 => $"{Base}/BodyMoveR.png",
                3 => $"{Base}/BodyMoveM.png",
                _ => ""
            });
        }

        public void SetAiming()
        {
            SetHeadTexture($"{Base}/HeadAiming.png");
            SetBodyTexture($"{Base}/BodyAiming.png");
        }

        private void SetBodyTexture(string path) => SetTextureOnEntity(Owner, path);
        private void SetHeadTexture(string path) => SetTextureOnEntity(HeadEntity, path);
        public void SetWeaponTexture(WeaponItem item)
        {
            if (WeaponEntity.TryGet<GSprite>(out var sprite)) sprite
                    .SetTexture(item.SavedTexture, false)
                    .SetSize(item.TexSize);
        }

        public void SetWeaponTexture()
        {
            if (WeaponEntity.TryGet<GSprite>(out var sprite)) sprite
                    .SetTexture(Owner.Scene.Assets.LoadTexture("None.png"), false);
        }

        private static void SetTextureOnEntity(Entity entity, string path)
        {
            var tex = entity.Scene.Assets.LoadTexture(path);
            if (entity.TryGet<GSprite>(out var sprite)) sprite
                    .SetTexture(tex, true);
        }

        public bool SetFacingDiecration(B2Vec2 dir)
        {
            bool isRight = dir.X > 0;
            Owner.Get<GSprite>()!.FlipX = !isRight;

            float side = isRight ? -1 : 1;
            pivot.X = PLAYER_SIZE.X * depth * side;
            float angle = MathF.Atan2(dir.Y, dir.X);

            HeadEntity.Transform.Rotation = angle;
            HeadEntity.Transform.Position2D = pivot;
            HeadEntity.Get<GSprite>()!
                .SetTranslation(side * new Vector2(pivot.X, 0))
                .FlipY = !isRight;

            var weaponSpriteDisp = side * new Vector2(pivot.X * 4, 0.1f);
            WeaponEntity.Transform.Rotation = angle;
            WeaponEntity.Transform.Position2D = pivot;
            WeaponEntity.Get<GSprite>()!
                .SetTranslation(weaponSpriteDisp)
                .FlipY = !isRight;

            if (Owner.Get<GPlayer>()!.Weaponry.HeldWeapon is WeaponItem hw)
            {
                var moff = hw.MuzzleOffset / Owner.Scene.World.PixelPerMetre;
                HeldWeaponMuzzleOffset = pivot + MathUtils.Rotate(weaponSpriteDisp + moff, -angle);
            }

            return isRight;
        }

        private static Entity AddBodyPart(Entity body)
        {
            var e = body.Scene.SpawnEntity();
            e.AddComponent<GSprite>();
            e.Transform.SetParent(body.Transform);
            return e;
        }
    }
}
