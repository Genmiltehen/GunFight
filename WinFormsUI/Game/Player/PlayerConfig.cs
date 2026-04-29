using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsUI.Game.Config;

namespace WinFormsUI.Game.Player
{
    public class PlayerConfig : IIdentifilable
    {
        public string Id { get; set; } = "";

        public float Armor { get; set; }
        public float MaxHealth { get; set; }
        public float HealthRegenRate { get; set; }

        public float Speed { get; set; }
        public float Acceleration { get; set; }
        public float JumpPower { get; set; }

        public string TextureName { get; set; } = "";
        //public string FootstepSound { get; set; } = "";
    }
}
