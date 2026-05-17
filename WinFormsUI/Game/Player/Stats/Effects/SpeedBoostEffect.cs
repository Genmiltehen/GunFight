namespace WinFormsUI.Game.Player.Stats.Effects
{
    public class SpeedBoostEffect(float intensity) : Effect
    {
        public override float TopSpeed => base.TopSpeed * intensity;
        public override float Acceleration => base.Acceleration * intensity;
    }
}
