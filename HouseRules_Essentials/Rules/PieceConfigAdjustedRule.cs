﻿namespace HouseRules.Essentials.Rules
{
    using System.Collections.Generic;
    using System.Linq;
    using Boardgame;
    using HarmonyLib;
    using HouseRules.Types;
    using UnityEngine;

    public sealed class PieceConfigAdjustedRule : Rule, IConfigWritable<List<List<string>>>, IMultiplayerSafe
    {
        public override string Description => "Piece configuration is adjusted";

        private readonly List<List<string>> _adjustments;

        /// <summary>
        /// Initializes a new instance of the <see cref="PieceConfigAdjustedRule"/> class.
        /// </summary>
        /// <param name="adjustments">Lists of PieceNames, PropertyNames and Values
        /// added to their base. Negative numbers are allowed.</param>
        public PieceConfigAdjustedRule(List<List<string>> adjustments)
        {
            _adjustments = adjustments;
        }

        public List<List<string>> GetConfigObject() => _adjustments;

        protected override void OnPostGameCreated(GameContext gameContext)
        {
            var pieceConfigs = Resources.FindObjectsOfTypeAll<PieceConfig>();
            foreach (var item in _adjustments)
            {
                var pieceConfig = pieceConfigs.First(c => c.name.Equals($"PieceConfig_{item[0]}"));
                var property = Traverse.Create(pieceConfig).Property<int>(item[1]);
                property.Value += int.Parse(item[2]);
            }
        }

        protected override void OnDeactivate(GameContext gameContext)
        {
            var pieceConfigs = Resources.FindObjectsOfTypeAll<PieceConfig>();
            foreach (var item in _adjustments)
            {
                var pieceConfig = pieceConfigs.First(c => c.name.Equals($"PieceConfig_{item[0]}"));
                var property = Traverse.Create(pieceConfig).Property<int>(item[1]);
                property.Value -= int.Parse(item[2]);
            }
        }
    }
}
