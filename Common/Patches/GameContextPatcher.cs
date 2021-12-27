﻿using Boardgame;
using Boardgame.Ui.LobbyMenu;
using Common.States;
using HarmonyLib;
using MelonLoader;

namespace Common.Patches
{
    internal static class GameContextPatcher
    {
        internal static void Patch(HarmonyLib.Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(GameStartup), "InitializeGame"),
                postfix: new HarmonyMethod(typeof(GameContextPatcher), nameof(GameStartup_InitializeGame_Postfix)));
            
            harmony.Patch(
                original: typeof(Lobby).GetMethod("Init"),
                postfix: new HarmonyMethod(typeof(GameContextPatcher), nameof(Lobby_Init_Postfix)));
        }
        
        private static void GameStartup_InitializeGame_Postfix(GameStartup __instance)
        {
            var gameContext = Traverse.Create(__instance).Field<GameContext>("gameContext").Value;
            GameContextState.GameContext = gameContext;
            GameContextState.AvatarController = gameContext.avatarController;
            GameContextState.CardHandController = gameContext.cardHandController;
            GameContextState.GameDataAPI = gameContext.gameDataAPI;
            GameContextState.PieceAndTurnController = gameContext.pieceAndTurnController;
            MelonLogger.Msg("Captured GameContext values.");
        }

        private static void Lobby_Init_Postfix(LobbyMenuController lobbyMenuController)
        {
            GameContextState.LobbyMenuController = lobbyMenuController;
            MelonLogger.Msg("Captured LobbyMenuController.");
        }
    }
}