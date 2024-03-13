using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using Game.DataStructures.Network;

namespace Game.Actors.Components
{
    public class PlayerControllerBasic : ActorEngine
    {
        [Header("Movement")]
        [SerializeField] private float MoveSpeed = 10;

        [Header("Jumping")]
        [SerializeField] private float JumpStrength = 5;
        [SerializeField] private float JumpGravityTimer = 1f;

        [Header("Gravity")]
        [SerializeField] private float Gravity = 9.8f;
        [SerializeField] private float JumpGravityMulti = 2.5f;
        [SerializeField] private float FallGravityMulti = 5f;

        [Header("Grounded Checks")]
        [SerializeField] private float HeadHeight = 2;
        [SerializeField] private LayerMask GroundedMask;

        [Header("Input")]
        [SerializeField] private InputAction JumpAction;
        [SerializeField] private InputAction DashAction;

        private bool IsGrounded = false;
        private bool JumpQueued = false;

        // TODO: with fish net, we need to seperate input and used variables
        /* example case
         We used input direction to fill moveinput of data on client
         In replication, we tried to fill input direction with moveinpunt
         this caused input direction to always be Zero'd on client
         */
        private Vector3 MoveInput; 

        private Vector3 LocalMoveDir;

        protected override void OnAwake()
        {
            base.OnAwake();
            Assert.IsNotNull(RigidBody);
        }

        private void OnEnable()
        {
            HeadTrans.localPosition = Vector3.up * HeadHeight;

            CurrentRotation = new Vector3(HeadTrans.localRotation.x, BodyTrans.localRotation.y, 0);

            JumpAction.Enable();
            JumpAction.performed += StartJump;

            DashAction.Enable();
            DashAction.performed += StartDash;
        }

        private void OnDisable()
        {
            JumpAction.performed -= StartJump;
            JumpAction.Disable();

            DashAction.performed -= StartDash;
            DashAction.Disable();
        }

        public override void Setup(in bool isOwner, in bool isServer, in bool isHost)
        {
            base.Setup(isOwner, isServer, isHost);

            if (!isOwner)
            {
                JumpAction.Disable();
                JumpAction.performed -= StartJump;

                DashAction.Disable();
                DashAction.performed -= StartDash;
            }
        }

        private void StartJump(InputAction.CallbackContext ctx)
        {
            if (!IsGrounded) return;
            JumpQueued = true;
        }

        private void StartDash(InputAction.CallbackContext ctx)
        {
            RigidBody.AddExplosionForce(50, this.transform.position + this.transform.right, 10, 0, ForceMode.VelocityChange);
        }

        private float ExternalForceTimer = 0;
        private void ApplyGravity(in float deltaTime)
        {
            if (!IsGrounded)
            {
                if (ExternalForceTimer > 0) 
                {
                    ExternalForceTimer -= deltaTime;
                    RigidBody.AddForce(Vector3.down * Gravity * JumpGravityMulti, ForceMode.Acceleration);
                }
                else
                {
                    RigidBody.AddForce(Vector3.down * Gravity * FallGravityMulti, ForceMode.Acceleration);
                }
            }
        }

        Ray GroundRay = new Ray();
        RaycastHit GroundHit;
        private void SetGrounded(in float deltaTime)
        {
            GroundRay.origin = HeadTrans.position;
            GroundRay.direction = Vector3.down;

            // if (Physics.Raycast(GroundRay, out RaycastHit hit, Mathf.Abs(RB.velocity.y) + Stats.HeadHeight, Stats.GroundedMask))
            if (Physics.Raycast(GroundRay, out GroundHit, HeadHeight + 0.1f, GroundedMask, QueryTriggerInteraction.Ignore))
            {
                IsGrounded = true;
            }
            else
            {
                IsGrounded = false;
            }
        }

        public override void OnPhysicsUpdate(in float deltaTime)
        {
            SetGrounded(deltaTime);
            Move(deltaTime);
            ApplyGravity(deltaTime);
        }

        private void Move(in float deltaTime)
        {
            LocalMoveDir.x = MoveInput.x * MoveSpeed;
            LocalMoveDir.z = MoveInput.z * MoveSpeed;
            RigidBody.AddForce(BodyTrans.rotation * LocalMoveDir * deltaTime, ForceMode.VelocityChange);
        }
        #region Networked Movement

        #region Replicate Input
        protected override void ConstructInputData(out InputData input)
        {
            input = new InputData(InputDirection, CombinedInputRotation, JumpQueued);
            CombinedInputRotation = Vector2.zero;
            JumpQueued = false;
        }

        protected override void ApplyInputData(InputData input)
        {
            MoveInput = input.MoveInput;
            InputRotation = input.RotInput;

            if (!base.IsOwner)
            {
                OnFrameUpdate((float)TimeManager.TickDelta);
                InputRotation = Vector3.zero;
            }

            OnPhysicsUpdate((float)TimeManager.TickDelta);
            if (input.JumpDown /*&& IsGrounded*/) // TODO : isgrounded check here causes clientside jitter on jump
            {
                ExternalForceTimer = JumpGravityTimer;
                RigidBody.AddForce(Vector3.up * JumpStrength, ForceMode.VelocityChange);
            }
        }
        #endregion
        #region Reconcile State
        protected override void GetStateData(out StateData state)
        {
            state = new StateData()
            {
                Position = BodyTrans.position,
                Rotation = new Vector3(HeadTrans.eulerAngles.x, BodyTrans.eulerAngles.y, 0),
                Velocity = RigidBody.velocity,
            };
        }

        protected override void ApplyStateData(StateData state)
        {
            BodyTrans.position = state.Position;
            RigidBody.velocity = state.Velocity;
            if (!base.IsOwner)
            {
                BodyTrans.localRotation = Quaternion.Euler(0, state.Rotation.y, 0);
                HeadTrans.localRotation = Quaternion.Euler(state.Rotation.x, 0, 0);
            }
        }
        #endregion

        #endregion
    }
}
