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
        [SyncVar] public string PlayerID;
        [SyncVar] public bool IsReady;
        #endregion
        [SyncVar]
        public Actor ControlledActor;

        #region NetworkCallbacks
        public override void OnStartServer()
        {
            base.OnStartServer();

            GameManager.Instance.Players.Add(this);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            GameManager.Instance.Players.Remove(this);
        }
        #endregion

        private void Update()
        {
            if (!IsOwner) return;
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (this.IsReady) ServerSetNotReady();
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


            RpcSetControlledActor(base.Owner, actorInstance);
        }

        [Server]
        public void StopGame()
        {
            if (ControlledActor != null && ControlledActor.IsSpawned) ControlledActor.DespawnActor();
        }

        [ServerRpc]
        void ServerSetReady()
        {
            IsReady = true;
            GameManager.Instance.PlayerReadyCheck();
        }
        [ServerRpc]
        void ServerSetNotReady()
        {
            IsReady = false;
            GameManager.Instance.PlayerReadyCheck();
        }

        [TargetRpc]
        private void RpcSetControlledActor(NetworkConnection conn, GameObject actorInstance)
        {
            ControlledActor = actorInstance.GetComponent<Actor>();
            ControlledActor.SetupActor(this.IsOwner, this.IsServer, this.IsHost);
        }
    }
}
