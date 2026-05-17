namespace WinFormsUI.Game.Player.Stats.Effects
{
    internal class ArmorBoostEffect(float intensity) : Effect
    {
        public override float Armor => base.Armor * intensity;
    }
}
