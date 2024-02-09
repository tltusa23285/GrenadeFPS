using FishNet.Component.Prediction;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using Game.Actors.Data;
using Game.DataStructures.Network;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace Game.Actors.Components
{
    /// <summary>
    /// Responsible for the physical movement of an actor
    /// </summary>
    public abstract class ActorEngine : NetworkBehaviour
    {
        [SerializeField] protected Camera Camera;
        [SerializeField] protected Transform HeadTrans;
        [SerializeField] protected Transform BodyTrans;
        protected Transform CameraTrans;


        protected Vector3 InputDirection = Vector3.zero;
        protected Vector3 InputRotation = Vector3.zero;

        protected Vector3 CombinedInputRotation = Vector3.zero; // sum of input rotation since last network tick

        [SerializeField] protected ActorEngineStats Stats;

        private void Awake()
        {
            Assert.IsNotNull(HeadTrans);
            Assert.IsNotNull(Camera);
            Assert.IsNotNull(BodyTrans);

            CameraTrans = Camera.transform;
            CameraTrans.position = HeadTrans.position;
            CameraTrans.rotation = HeadTrans.rotation;
            Camera.enabled = false;

            OnAwake();
        }

        public virtual void Setup(in bool isOwner, in bool isServer, in bool isHost)
        {
            if (!isOwner)
            {
                Camera.enabled = false;
                if (Camera.TryGetComponent(out AudioListener a))
                {
                    Destroy(a);
                }
            }
            else
            {
                Camera.enabled = true;
                Camera.gameObject.AddComponent<AudioListener>();
            }
        }


        protected virtual void OnAwake()
        {
            InputRotation = new Vector3(HeadTrans.eulerAngles.x, BodyTrans.eulerAngles.y, 0);
        }


        /// <summary>
        /// Normalizes given direction and moves actor in direction based on speed
        /// </summary>
        /// <param name="input"></param>
        public virtual void SetInputDirection(Vector2 input)
        {
            InputDirection.x = input.normalized.x;
            InputDirection.z = input.normalized.y;
        }

        /// <summary>
        /// Normalizes given ipnut and rotates based on look speed
        /// Internally swizzles values as needed
        /// </summary>
        /// <param name="input"></param>
        public virtual void SetInputRotation(Vector2 input)
        {
            InputRotation.x = input.y * Stats.LookSpeed;
            InputRotation.y = input.x * Stats.LookSpeed;

            CombinedInputRotation += InputRotation;
        }
        public virtual void OnFrameUpdate(in float deltaTime) { }
        public virtual void OnPhysicsUpdate(in float deltaTime) { }

        private void LateUpdate()
        {
            CameraTrans.rotation = HeadTrans.rotation;
        }

        #region Networked Movement
        private bool IsSubbedToTime;

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            SubscribeToTimeManager(true);
        }

        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
            SubscribeToTimeManager(false);
        }

        public void SubscribeToTimeManager(bool subscribe)
        {
            if (base.TimeManager == null) return;
            if (subscribe == IsSubbedToTime) return;

            IsSubbedToTime = subscribe;

            if (subscribe)
            {
                base.TimeManager.OnTick += TimeManager_OnTick;
                base.TimeManager.OnPostTick += TimeManager_OnPostTick;
            }
            else
            {
                base.TimeManager.OnTick -= TimeManager_OnTick;
                base.TimeManager.OnPostTick -= TimeManager_OnPostTick;
            }
        }

        // TODO : ideally this would be handled more abstracly, however Fish-Net currently does not support generics or protected methods for reconcile/replicate

        // occurs before fixed update
        protected virtual void TimeManager_OnTick()
        {
            if (base.IsOwner)
            {
                ReconcileState(default, false);
                GetInputData(out InputData input);
                ReplicateInput(input, false);
            }
            if (base.IsServer) ReplicateInput(default, true);
        }
        // occurs after fixed update
        protected virtual void TimeManager_OnPostTick()
        {
            if (base.IsServer)
            {
                GetStateData(out StateData state);
                ReconcileState(state, true);
            }
        }

        [Reconcile]
        private void ReconcileState(StateData state, bool asServer, Channel channel = Channel.Unreliable)
        {
            ApplyStateData(state);
        }
        [Replicate]
        private void ReplicateInput(InputData input, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
        {
            ApplyInputData(input);
        }

        protected virtual void GetInputData(out InputData input)
        {
            input = new InputData()
            {
                MoveInput = InputDirection,
                RotInput = CombinedInputRotation
            };
            CombinedInputRotation = Vector2.zero;
        }
        protected virtual void ApplyInputData(InputData input) 
        {
            InputDirection = input.MoveInput;
            InputRotation = input.RotInput;

            if (!base.IsOwner)
            {
                OnFrameUpdate((float)TimeManager.TickDelta);
            }
            OnPhysicsUpdate((float)TimeManager.TickDelta);
        }

        protected virtual void GetStateData(out StateData state)
        {
            state = new StateData()
            {
                Position = BodyTrans.position,
                Rotation = new Vector3(HeadTrans.eulerAngles.x, BodyTrans.eulerAngles.y, 0)
            };
        }
        protected virtual void ApplyStateData(StateData state)
        {
            transform.position = state.Position;
            BodyTrans.rotation = Quaternion.Euler(0, state.Rotation.y, 0);
            HeadTrans.rotation = Quaternion.Euler(state.Rotation.x, 0, 0);
        }
        #endregion
    } 
}
