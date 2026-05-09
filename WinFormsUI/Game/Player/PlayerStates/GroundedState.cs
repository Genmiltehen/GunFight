using XEngine.Core.Scenery;
using static WinFormsUI.Game.Player.Contol.ActionType;
using static WinFormsUI.Game.Box2D.ContactFlags;
using Box2D.NET;
using XEngine.Core.Box2DCompat;
using System.Diagnostics;
using WinFormsUI.Game.Drop;

namespace WinFormsUI.Game.Player.PlayerStates
{
    internal class GroundedState : IPlayerState
    {
        public string DebugName => "grounded";

        public void Enter(GPlayer player, GScene scene)
        {
            if (player.Control.HorizotnalInput() == 0) player.Model.SetIdling();
            else player.Model.SetMoving();
        }

        public void Exit(GPlayer player, GScene scene) { }

        public void ProcessInput(GPlayer player, GScene scene, float dt)
        {
            if (player.Control.Fetch("up", ActionActive) && player.Contacts.Has(LADDER))
            {
                player.SwitchTo<ClimbState>(scene);
                return;
            }

            if (player.Control.Fetch("up", ActionStart))
            {
                player.JumpTimer.Start();
                player.Body.ApplyImpulse(0, player.Stats.JumpPower);
            }

            if (player.Control.Fetch("aux", ActionStart))
            {
                player.Weaponry.Swap();
                player.Model.UpdatePockets(player.Weaponry);
            }

            if (player.Control.Fetch("act", ActionStart))
            {
                var shapeid = player.Contacts.Get(ITEM_HITBOX).FirstOrDefault();
                if (player.Weaponry.HeldWeapon == null &&
                    B2Worlds.b2Shape_IsValid(shapeid) &&
                    B2Helpers.TryFetchEntity(shapeid, out var dropEntity) &&
                    dropEntity.TryGet<GDrop>(out var drop) &&
                    drop.TryTake(out var weapon))
                {
                    player.Weaponry.Equip(weapon);
                    player.Model.UpdatePockets(player.Weaponry);
                }

                // TODO: Pick up
            }

            float hor = player.Control.HorizotnalInput();
            if (hor == 0) player.Model.SetIdling();
            else
            {
                player.Model.SetMoving();
                player.IsRightFacing = player.Model.SetFacingDiecration(hor);
                player.Move(hor, dt);
            }

            if (!player.Contacts.Has(SOLID)) player.SwitchTo<FallState>(scene);
            else if (hor == 0 && player.Control.Fetch("aim", ActionActive)) player.SwitchTo<AimState>(scene);
        }
    }
}
