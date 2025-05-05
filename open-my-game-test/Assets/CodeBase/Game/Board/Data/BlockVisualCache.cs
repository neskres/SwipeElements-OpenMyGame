using System.Collections.Generic;
using System.Linq;
using Game.Shared.StaticData;
using UnityEngine;
using Zenject;

namespace Game.Board.Data
{
    public class BlockVisualCache : IInitializable
    {
        private readonly RuntimeAnimatorController _baseCtrl;
        private readonly IEnumerable<BlockVisualSO> _visuals;

        private readonly Dictionary<BlockType, AnimatorOverrideController> _map = new();

        [Inject]
        public BlockVisualCache(
            [Inject(Id = "BaseBlock")] RuntimeAnimatorController baseCtrl,
            IEnumerable<BlockVisualSO> visuals)
        {
            _baseCtrl = baseCtrl;
            _visuals = visuals;
        }

        public void Initialize()
        {
            foreach (var v in _visuals)
            {
                var ovr = new AnimatorOverrideController(_baseCtrl);

                ovr["Idle"] = v.IdleClip;
                ovr["Destroy"] = v.DestroyClip;
                _map[v.Type] = ovr;
            }
        }

        public AnimatorOverrideController GetOverride(BlockType t) => _map[t];
    }
}