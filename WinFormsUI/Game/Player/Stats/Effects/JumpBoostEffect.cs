namespace WinFormsUI.Game.Player.Stats.Effects
{
    internal class JumpBoostEffect(float intensity) : Effect
    {
        public override float JumpPower => base.JumpPower * intensity;
    }
}
