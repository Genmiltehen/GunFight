using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XEngine.Core.Input;

namespace WinFormsUI.Game.Player
{
    public static class PlayerControlHelper
    {
        public static bool IsOnGround(GPlayer player) => player.GroundContacts > 0;

        public static float HorizotnalInput(GPlayer player, IInputService input)
        {
            return input.GetAxis($"Horizontal{player.Name}");
        }

        public static float VerticalInput(GPlayer player, IInputService input)
        {
            return input.GetAxis($"Vertical{player.Name}");
        }

        // --- shooting / aiming ---
        public static bool AimStart(GPlayer player, IInputService input)
        {
            return input.IsActionJustPressed($"shoot{player.Name}");
        }

        public static bool Aiming(GPlayer player, IInputService input)
        {
            return input.IsActionActive($"shoot{player.Name}");
        }

        public static bool AimEnd(GPlayer player, IInputService input)
        {
            return input.IsActionJustReleased($"shoot{player.Name}");
        }

        // --- jumping ---
        public static bool JumpStart(GPlayer player, IInputService input)
        {
            return input.IsActionJustPressed($"jump{player.Name}");
        }

        public static bool Jumping(GPlayer player, IInputService input)
        {
            return input.IsActionActive($"jump{player.Name}");
        }

        public static bool JumpEnd(GPlayer player, IInputService input)
        {
            return input.IsActionJustReleased($"jump{player.Name}");
        }
    }
}
