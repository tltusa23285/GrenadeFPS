using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Game.Actors;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Game.Managers;
using UnityEngine.AddressableAssets;
using FishNet.Connection;
using FishNet;

namespace Game
{
    public class Player : NetworkBehaviour
    {
        #region Sync Variables
        [Header("Synced")]
        public readonly SyncVar<string> PlayerID = new();
        public readonly SyncVar<bool> IsReady = new();
        public readonly SyncVar<Actor> ControlledActor = new();
        #endregion

        #region NetworkCallbacks
        public override void OnStartServer()
        {
            base.OnStartServer();

            InstanceFinder.GetInstance<GameManager>().Players.Add(this);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            InstanceFinder.GetInstance<GameManager>().Players.Remove(this);
        }
        #endregion

        private void Awake()
        {
            ControlledActor.OnChange += OnControlledActorChange;
        }

        private void Update()
        {
            if (!IsOwner) return;
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (this.IsReady.Value) ServerSetNotReady();
                else ServerSetReady();
                }

            //ControlledActor?.OnUpdate();
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;

            //ControlledActor?.OnFixedUpdate();
        }

        [Server]
        public void StartGame()
        {
            GameObject actor = Addressables.LoadAssetAsync<GameObject>("Actor").WaitForCompletion();

            GameObject actorInstance = Instantiate(actor);

            Spawn(actorInstance, Owner);

            ControlledActor.Value = actorInstance.GetComponent<Actor>();
        }

        [Server]
        public void StopGame()
        {
            if (ControlledActor.Value != null && ControlledActor.Value.IsSpawned) ControlledActor.Value.DespawnActor();
        }

        [ServerRpc]
        void ServerSetReady()
        {
            IsReady.Value = true;
            InstanceFinder.GetInstance<GameManager>().PlayerReadyCheck();
        }
        [ServerRpc]
        void ServerSetNotReady()
        {
            IsReady.Value = false;
            InstanceFinder.GetInstance<GameManager>().PlayerReadyCheck();
        }
        private void OnControlledActorChange(Actor prev, Actor next, bool asServer)
        {
            ControlledActor.Value.SetupActor(this.IsOwner, this.IsServerInitialized, this.IsHostInitialized);
        }
    }
}
