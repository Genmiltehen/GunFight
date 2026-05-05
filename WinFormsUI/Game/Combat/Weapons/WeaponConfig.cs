using OpenTK.Mathematics;
using WinFormsUI.Game.Config;
using XEngine.Core.Utils.JSONConverters;

namespace WinFormsUI.Game.Combat.Weapons
{
    public class WeaponConfig : IIdentifilable
    {
        public string Id { get; set; } = "";
        public string ProjectileId { get; set; } = "";

        public int MaxAmmo { get; set; }
        public float FireRate { get; set; }
        public float Spread { get; set; }
        public int Shots { get; set; }
        public float InitialVelocity { get; set; }

        public float TextureScale { get; set; }

        [JsonVector2]
        public Vector2 MuzzleOffsetRatio { get; set; } = Vector2.Zero;
        public string TexturePath { get; set; } = "None.png";
        //public string FireSoundPath { get; set; } = "";
    }
}
