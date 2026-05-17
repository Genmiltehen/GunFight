namespace WinFormsUI.Game.Box2D
{
    [Flags]
    public enum ContactFlags : ulong
    {
        None = 0,
        SOLID = 1 << 0,
        PLAYER = 1 << 1,
        FOOT = 1 << 2,
        PROJECTILE = 1 << 3,
        ITEM = 1 << 4,
        ITEM_HITBOX = 1 << 5,
        EFFECT = 1 << 6,
        LADDER = 1 << 7,
    }
}
