using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using XEngine.Core.Base;
using XEngine.Core.Graphics.OpenGL;
using XEngine.Core.Scenery;
using XEngine.Core.Utils;

namespace WinFormsUI.Game.Combat.Weapons
{
    public class WeaponItem
    {
        private readonly WeaponConfig _config;
        public readonly GameTimer FireTimer = new(float.MaxValue);

        public string ProjectileId => _config.ProjectileId;
        public float Spread => _config.Spread;
        public float MaxAmmo => _config.MaxAmmo;
        public float InitialVelocity => _config.InitialVelocity;
        public int Shots => _config.Shots;


        public float CurrentAmmo = 0;

        public Vector2 MuzzleOffset { get; private set; } = Vector2.Zero;
        public Vector2 TexSize { get; private set; } = Vector2.Zero;
        public Texture2D SavedTexture { get; private set; } = null!;
        private bool _initialized = false;

        public WeaponItem(WeaponConfig config)
        {
            _config = config;
            FireTimer.Duration = 1.0f / config.FireRate;
            CurrentAmmo = MaxAmmo;
        }

        public WeaponItem InitTexture(IAssetLoader assets)
        {
            if (_initialized) return this;
            _initialized = true;

            SavedTexture = assets.LoadTexture(_config.TexturePath);
            TexSize = SavedTexture.Size * _config.TextureScale;
            MuzzleOffset = _config.MuzzleOffsetRatio * TexSize;
            return this;
        }
    }
}
