namespace Game.Board.Data
{
    public readonly struct BoardCoordinates
    {
        public readonly int X;
        public readonly int Y;

        public BoardCoordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"({X},{Y})";
    }
}