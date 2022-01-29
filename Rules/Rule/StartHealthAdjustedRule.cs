namespace Rules.Rule
{
    using System.Collections.Generic;
    using System.Linq;
    using Boardgame;
    using HarmonyLib;
    using UnityEngine;

    public sealed class StartHealthAdjustedRule : RulesAPI.Rule
    {
        public override string Description => "Piece Starting Health is adjusted";

        private readonly Dictionary<string, int> _adjustments;

        public StartHealthAdjustedRule(Dictionary<string, int> adjustments)
        {
            _adjustments = adjustments;
        }

        protected override void OnActivate()
        {
            var pieceConfigs = Resources.FindObjectsOfTypeAll<PieceConfig>();

            foreach (var item in _adjustments)
            {
                var pieceConfig = pieceConfigs.First(c => c.name.Equals($"PieceConfig_{item.Key}"));
                var actionPoint = Traverse.Create(pieceConfig).Property<int>("StartHealth");
                actionPoint.Value += item.Value;
            }
        }

        protected override void OnDeactivate()
        {
            var pieceConfigs = Resources.FindObjectsOfTypeAll<PieceConfig>();

            foreach (var item in _adjustments)
            {
                var pieceConfig = pieceConfigs.First(c => c.name.Equals($"PieceConfig_{item.Key}"));
                var actionPoint = Traverse.Create(pieceConfig).Property<int>("StartHealth");
                actionPoint.Value -= item.Value;
            }
        }
    }

}

