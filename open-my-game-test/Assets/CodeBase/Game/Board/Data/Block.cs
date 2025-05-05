using Game.Shared.StaticData;

namespace Game.Board.Data
{
    public readonly struct Block : IBlock
    {
        public BlockType Type { get; }

        public Block(BlockType type)
        {
            Type = type;
        }
    }
}