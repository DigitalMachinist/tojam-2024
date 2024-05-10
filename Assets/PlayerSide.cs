public enum PlayerSide : int
{
    Left,
    Right
}

static class PlayerSideExtensions 
{
    public static PlayerSide Opposite(this PlayerSide side)
    {
        return side == PlayerSide.Left
            ? PlayerSide.Right
            : PlayerSide.Left;
    }
}