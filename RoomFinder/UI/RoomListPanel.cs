﻿using System;
using System.Collections.Generic;
using Boardgame;
using Common.States;
using Common.Ui;
using HarmonyLib;
using MelonLoader;
using Photon.Realtime;
using UnityEngine;

namespace RoomFinder.UI
{
    internal class RoomListPanel
    {
        private readonly GameObject panelObject;

        public static RoomListPanel NewInstance()
        {
            return new RoomListPanel();
        }
        
        private RoomListPanel()
        {
            panelObject = new GameObject("RoomListPanel");
        }

        public GameObject Reinitialize(List<RoomInfo> rooms)
        {
            foreach (Transform child in panelObject.transform)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
            
            RenderHeader();
            for (var i = 0; i < rooms.Count; i++)
            {
                RenderRoomRow(rooms[i], i);
            }

            return panelObject;
        }

        private void RenderHeader()
        {
            var headerContainer = new GameObject("Header");
            headerContainer.transform.SetParent(panelObject.transform, worldPositionStays: false);
            
            var joinLabel = UiUtil.CreateLabelText("Code");
            joinLabel.transform.SetParent(headerContainer.transform, worldPositionStays: false);
            joinLabel.transform.localPosition = new Vector3(-3f, 0, 0);
            
            var gameLabel = UiUtil.CreateLabelText("Game");
            gameLabel.transform.SetParent(headerContainer.transform, worldPositionStays: false);
            gameLabel.transform.localPosition = new Vector3(-0.2f, 0, 0);
            
            var floorLabel = UiUtil.CreateLabelText("Floor");
            floorLabel.transform.SetParent(headerContainer.transform, worldPositionStays: false);
            floorLabel.transform.localPosition = new Vector3(1.75f, 0, 0);
            
            var playersLabel = UiUtil.CreateLabelText("Players");
            playersLabel.transform.SetParent(headerContainer.transform, worldPositionStays: false);
            playersLabel.transform.localPosition = new Vector3(3.5f, 0, 0);
        }

        private void RenderRoomRow(RoomInfo room, int row)
        {
            var yOffset = (1 + row) * -1f;
            var roomRowContainer = new GameObject($"Row{row}");
            roomRowContainer.transform.SetParent(panelObject.transform, worldPositionStays: false);
            roomRowContainer.transform.localPosition = new Vector3(0, yOffset, 0);

            object obj;
            var gameType = room.CustomProperties.TryGetValue("at", out obj)
                ? (LevelSequence.GameType) obj
                : LevelSequence.GameType.Invalid;
            var floorIndex = room.CustomProperties.TryGetValue("fi", out obj) ? (int) obj : -1;

            if (gameType == LevelSequence.GameType.Invalid || floorIndex < 0) return;

            var joinButton = UiUtil.CreateButton(JoinRoomAction(room.Name));
            joinButton.transform.SetParent(roomRowContainer.transform, worldPositionStays: false);
            joinButton.transform.localScale = new Vector3(0.4f, 0.7f, 0.7f);
            joinButton.transform.localPosition = new Vector3(-3f, 0, 0);
            
            var joinText = UiUtil.CreateText(room.Name, Color.white, UiUtil.DefaultLabelFontSize);
            joinText.transform.SetParent(roomRowContainer.transform, worldPositionStays: false);
            joinText.transform.localPosition = new Vector3(-3f, 0, 0);

            var gameName = StringifyGameType(gameType);
            var gameLabel = UiUtil.CreateLabelText(gameName);
            gameLabel.transform.SetParent(roomRowContainer.transform, worldPositionStays: false);
            gameLabel.transform.localPosition = new Vector3(-0.2f, 0, 0);
            
            var floorLabel = UiUtil.CreateLabelText(floorIndex.ToString());
            floorLabel.transform.SetParent(roomRowContainer.transform, worldPositionStays: false);
            floorLabel.transform.localPosition = new Vector3(1.5f, 0, 0);
            
            var playersText = $"{room.PlayerCount}/{room.MaxPlayers}";
            var playersLabel = UiUtil.CreateLabelText(playersText);
            playersLabel.transform.SetParent(roomRowContainer.transform, worldPositionStays: false);
            playersLabel.transform.localPosition = new Vector3(3.25f, 0, 0);
        }

        private static string StringifyGameType(LevelSequence.GameType gameType)
        {
            switch (gameType)
            {
                case LevelSequence.GameType.ElvenQueen:
                    return "BS";
                case LevelSequence.GameType.RatKing:
                    return "RK";
                case LevelSequence.GameType.Forest:
                    return "RoE";
                default:
                    return "Unknown";
            }
        }

        private static Action JoinRoomAction(string roomCode)
        {
            return () =>
            {
                MelonLogger.Msg($"Joining room [{roomCode}].");
                Traverse.Create(GameContextState.LobbyMenuController).Method("JoinGame", roomCode, true).GetValue();
            };
        }
    }
}