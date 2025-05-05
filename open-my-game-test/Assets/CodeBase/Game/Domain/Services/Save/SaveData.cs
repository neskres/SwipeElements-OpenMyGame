using System;
using Game.Shared.StaticData;

namespace Game.Domain.Services.Save
{
    [Serializable]
    public class SaveData
    {
        public int CurrentLevelIndex;
        public BlockType[][] Board;
    }
}