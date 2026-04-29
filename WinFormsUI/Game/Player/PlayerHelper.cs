using Box2D.NET;
using OpenTK.Mathematics;
using WinFormsUI.Game.Box2D;
using WinFormsUI.Game.Player.Stats;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Common;
using XEngine.Core.Common.Sprite;
using XEngine.Core.Scenery;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace WinFormsUI.Game.Player
{
    public static class PlayerHelper
    {
        public static readonly B2Vec2 PLAYER_SIZE = new(1f, 2f);
        public static readonly Vector2 PIVOT_POINT_RIGHT = new(PLAYER_SIZE.X * depth, PLAYER_SIZE.Y * 0.8f);
        public static readonly Vector2 PIVOT_POINT_LEFT = new(PLAYER_SIZE.X * -depth, PLAYER_SIZE.Y * 0.8f);
        public const float depth = 0.3f;

        public static GPlayer SetFacing(GPlayer player, B2Vec2 dir, bool forceRecalc = false)
        {
            float angle = B2MathFunction.b2Atan2(dir.Y, dir.X);
            player.HeadEntity.Transform.Rotation = angle;
            player.WeaponEntity.Transform.Rotation = angle;

            if (player.IsRightFacing != dir.X > 0 || forceRecalc)
            {
                player.IsRightFacing = dir.X > 0;
                player.BodyEntity.Get<GSprite>()!.FlipX = !player.IsRightFacing;
                SetHeadWeaponPivot(player, player.IsRightFacing);
            }
            return player;
        }

        private static void SetHeadWeaponPivot(GPlayer player, bool isRight)
        {
            var hSprite = player.HeadEntity.Get<GSprite>()!;
            var wSprite = player.WeaponEntity.Get<GSprite>()!;
            if (isRight)
            {
                player.HeadEntity.Transform.Position2D = PIVOT_POINT_LEFT;
                player.WeaponEntity.Transform.Position2D = PIVOT_POINT_LEFT;
                hSprite.SetTranslation(new(-PIVOT_POINT_LEFT.X, 0));
                wSprite.SetTranslation(new(-PIVOT_POINT_LEFT.X * 2, 0));
            }
            else
            {
                player.HeadEntity.Transform.Position2D = PIVOT_POINT_RIGHT;
                player.WeaponEntity.Transform.Position2D = PIVOT_POINT_RIGHT;
                hSprite.SetTranslation(new(PIVOT_POINT_RIGHT.X, 0));
                wSprite.SetTranslation(new(PIVOT_POINT_RIGHT.X * 2, 0));
            }
            wSprite.FlipY = !isRight;
            hSprite.FlipY = !isRight;
        }
    }
}
