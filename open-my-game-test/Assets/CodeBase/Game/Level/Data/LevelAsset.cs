using CodeBase.Game.Board;
using Game.Board.Data;
using Game.Shared.StaticData;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Level.Data
{
    [CreateAssetMenu(menuName = "Match-3/Level Asset")]
    public class LevelAsset : ScriptableObject
    {
        [FormerlySerializedAs("Width")] [Min(1)]
        public int width = 6;

        [FormerlySerializedAs("Height")] [Min(1)]
        public int height = 6;

        [Tooltip("Строки сверху вниз. Символы: F W E A T = Fire Water Earth Air Ether")]
        public string[] Rows;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Rows == null || Rows.Length != height)
                Rows = new string[height];

            for (int y = 0; y < height; y++)
                if (Rows[y] == null || Rows[y].Length != width)
                    Rows[y] = new string('F', width);
        }
#endif

        public BoardState ToBoardState()
        {
            var grid = new Block[width, height];

            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                grid[x, height - 1 - y] = new Block(CharToType(Rows[y][x]));

            return new BoardState(grid);
        }

        private BlockType CharToType(char c) => c switch
        {
            'F' => BlockType.Fire,
            'W' => BlockType.Water,
            '.' => BlockType.Empty,
            _ => BlockType.Empty
        };
    }
}