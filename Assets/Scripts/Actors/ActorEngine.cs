using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using Game.DataStructures.Network;
using UnityEngine;
using UnityEngine.Assertions;
using Game.Interfaces;

namespace Game.Actors.Components
{
    /// <summary>
    /// Responsible for the physical movement of an actor
    /// </summary>
    public abstract class ActorEngine : NetworkBehaviour, IForceable
    {
        [Header("References")]
        [SerializeField] protected Rigidbody RigidBody;
        [SerializeField] protected Camera Camera;
        [SerializeField] protected Transform HeadTrans;
        [SerializeField] protected Transform BodyTrans;

        protected Transform CameraTrans;

        [Header("Look Settings")]
        [SerializeField] protected float LookSpeed = 1;
        [SerializeField] protected float VertLookClamp = 85;

        protected Vector3 InputDirection = Vector3.zero;
        protected Vector3 InputRotation = Vector3.zero;

        protected Vector3 CombinedInputRotation = Vector3.zero; // sum of input rotation since last network tick
        protected Vector3 CurrentRotation;

        private void Awake()
        {
            Assert.IsNotNull(HeadTrans);
            Assert.IsNotNull(Camera);
            Assert.IsNotNull(BodyTrans);
            Assert.IsNotNull(RigidBody);

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
        public void SetInputDirection(Vector2 input)
        {
            InputDirection.x = input.normalized.x;
            InputDirection.z = input.normalized.y;
        }

        /// <summary>
        /// Normalizes given ipnut and rotates based on look speed
        /// Internally swizzles values as needed
        /// </summary>
        /// <param name="input"></param>
        public void SetInputRotation(Vector2 input)
        {
            InputRotation.x = input.y * LookSpeed;
            InputRotation.y = input.x * LookSpeed;

            CombinedInputRotation += InputRotation;
        }

        protected virtual void SetRotation(in Vector3 rotation)
        {
            CurrentRotation = rotation;
            CurrentRotation.x = Mathf.Clamp(CurrentRotation.x, -VertLookClamp, VertLookClamp);
            BodyTrans.localRotation = Quaternion.Euler(0, rotation.y, 0);
            HeadTrans.localRotation = Quaternion.Euler(rotation.x, 0, 0);

        }

        public virtual void OnFrameUpdate(in float deltaTime) 
        {
            SetRotation(CurrentRotation + InputRotation);
        }

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

        private void SubscribeToTimeManager(bool subscribe)
        {
            if (base.TimeManager == null) return;
            if (subscribe == IsSubbedToTime) return;

            IsSubbedToTime = subscribe;

            if (subscribe)
            {
                base.TimeManager.OnTick += TimeManager_OnTick;
                base.TimeManager.OnPostTick += TimeManager_OnPostTick;
                if (!base.IsOwner)
                {
                    base.PredictionManager.OnPreReconcile += PredictionManager_PreReconcile;
                    base.PredictionManager.OnPostReconcile += PredictionManager_PostReconcile;
                }
            }
            else
            {
                base.TimeManager.OnTick -= TimeManager_OnTick;
                base.TimeManager.OnPostTick -= TimeManager_OnPostTick;
                if (!base.IsOwner)
                {
                    base.PredictionManager.OnPreReconcile -= PredictionManager_PreReconcile;
                    base.PredictionManager.OnPostReconcile -= PredictionManager_PostReconcile;
                }   
            }
        }

        // TODO : ideally this would be handled more abstracly, however Fish-Net currently does not support generics or protected methods for reconcile/replicate

        #region Replicate Input

        // occurs before fixed update
        private void TimeManager_OnTick()
        {
            GetInputData(out InputData input);
            ReplicateInput(input);
        }
        [Replicate]
        private void ReplicateInput(InputData input, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
        {
            ApplyInputData(input);
        }

        private void GetInputData(out InputData input)
        {
            if (!base.IsOwner) input = default;
            else ConstructInputData(out input);
        }
        protected virtual void ConstructInputData(out InputData input)
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
        #endregion

        #region Reconcile State
        // occurs after fixed update
        private void TimeManager_OnPostTick()
        {
            if (base.IsServerInitialized)
            {
                GetStateData(out StateData state);
                ReconcileState(state);
            }
        }

        [Reconcile]
        private void ReconcileState(StateData state, Channel channel = Channel.Unreliable)
        {
            ApplyStateData(state);
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

        Vector3 SavedRot;

        protected virtual void PredictionManager_PreReconcile(uint clientTick, uint serverTick)
        {
            SavedRot = CurrentRotation;
        }

        protected virtual void PredictionManager_PostReconcile(uint clientTick, uint serverTick)
        {
            SetRotation(SavedRot);
        }
        #endregion

        #endregion

        #region IForceable
        public void AddForce(Vector3 force, ForceMode mode)
        {
            RigidBody.AddForce(force, mode);
        }

        public void AddExplosionForce(float force, Vector3 origin, float radius, ForceMode mode)
        {
            RigidBody.AddExplosionForce(force, origin, radius, 0, mode);
        } 
        #endregion
    } 
}
