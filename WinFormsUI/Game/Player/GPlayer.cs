using WinFormsUI.Game.Combat.Weapons;
using WinFormsUI.Game.Player.Components;
using WinFormsUI.Game.Player.Contol;
using WinFormsUI.Game.Player.PlayerStates;
using WinFormsUI.Game.Player.Stats;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat.Components;
using XEngine.Core.Scenery;
using XEngine.Core.Utils;

namespace WinFormsUI.Game.Player
{
    public class GPlayer : GameComponent
    {
        public string Name = "NullPlayer";
        public bool IsRightFacing { get; set; } = true;

        public readonly GameTimer JumpTimer = new(0.3f);
        public readonly GameTimer WalkTimer = new(0.15f);
        public PlayerEffectList Effects = null!;

        private IPlayerState? _state;
        public GPlayerWeaponry Weaponry { get; private set; } = null!;
        public GPlayerMovement Movement { get; private set; } = null!;
        public GPlayerModel Model { get; private set; } = null!;
        public PlayerControl Control { get; private set; } = null!;

        public GPlayer Init(GScene scene, PlayerConfig config)
        {
            Weaponry = Owner.AddComponent<GPlayerWeaponry>().Init(config.StartWeaponId);
            Movement = Owner.AddComponent<GPlayerMovement>().Init(Owner.Get<GBox2DBody>()!.Id);
            Model = Owner.AddComponent<GPlayerModel>().Init(config);
            IsRightFacing = Model.SetFacingDiecration(new(1, 0));

            Effects = new(new PlayerStats(config));
            _state = new IdleState();

            scene.RegisterTimer(JumpTimer);
            scene.RegisterTimer(WalkTimer);
            Control = new PlayerControl(scene.Input);
            return this;
        }

        public void ProcessInput(GScene scene, float dt)
        {
            _state?.ProcessInput(this, scene, dt);
        }

        public GPlayer SetName(string name)
        {
            Name = name;
            Control.Name = name;
            return this;
        }

        public IPlayerStats Stats => Effects.GetStats();

        public void SwitchTo<T>(GScene scene) where T : IPlayerState, new()
        {
            _state?.Exit(this, scene);
            _state = new T();
            _state.Enter(this, scene);
        }
    }
}
