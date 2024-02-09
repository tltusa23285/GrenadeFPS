using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Game.Actors;
using System.Linq;

namespace Game.Managers
{
    public class GameManager : NetworkBehaviour
    {
        #region MyRegion
        private Rect GuiRect;
        private float LineHeight = 20f;
        private int LineCount;
        private void OnGUI()
        {
            GuiRect.Set(
                Screen.width - 100f,
                0,
                100,
                LineHeight * LineCount
                );
            LineCount = 1;
            GUI.Box(GuiRect, "Server Stats");

            LineCount++;
            GuiRect.Set(GuiRect.x,GuiRect.y + LineHeight, GuiRect.width, LineHeight);
            GUI.Label(GuiRect,$"Players: {Players.Count}");

            LineCount++;
            GuiRect.Set(GuiRect.x, GuiRect.y + LineHeight, GuiRect.width, LineHeight);
            int i = 0;
            foreach (Player item in Players)
            {
                if (item.IsReady.Value)
                {
                    i++;
                }
            }
            GUI.Label(GuiRect, $"Ready: {i}/{Players.Count}");
        } 
        #endregion


        public static GameManager Instance { get; private set; }

        public readonly SyncList<Player> Players = new();

        public readonly SyncVar<bool> CanStart = new(false);

        private void Awake()
        {
            Instance = this;
        }

        [Server]
        public void PlayerReadyCheck()
        {
            foreach (Player item in Players)
            {
                if (!item.IsReady.Value) return;
            }

            StartGame();
        }

        [Server]
        public void StartGame()
        {
            foreach (Player item in Players) { item.StartGame(); }
        }

        [Server]
        public void StopGame()
        {
            foreach (Player item in Players) { item.StopGame(); }
        }
    }
}
