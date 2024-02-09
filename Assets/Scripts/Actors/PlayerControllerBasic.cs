using Game.Actors.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Interfaces;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using Game.DataStructures.Network;
using Game.DataStructures;
using FishNet.Object.Prediction;
using FishNet.Transporting;

namespace Game.Actors.Components
{
    public class PlayerControllerBasic : ActorEngine
    {
        [Header("Basic")]

        [Header("Input")]
        [SerializeField] private InputAction JumpAction;
        [SerializeField] private InputAction DashAction;

        [Header("References")]
        [SerializeField] private Rigidbody RB;

        private bool IsGrounded = false;
        private bool JumpQueued = false;

        // TODO: with fish net, we need to seperate input and used variables
        /* example case
         We used input direction to fill moveinput of data on client
         In replication, we tried to fill input direction with moveinpunt
         this caused input direction to always be Zero'd on client
         */
        private Vector3 MoveInput; 
        private Vector3 CurrentRotation;

        private Vector3 LocalMoveDir;
        protected override void OnAwake()
        {
            base.OnAwake();
            Assert.IsNotNull(RB);
        }

        private void OnEnable()
        {
            HeadTrans.localPosition = Vector3.up * Stats.HeadHeight;

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
            //LocalMoveDir.y = Stats.JumpStrength;
            //RB.AddForce(Vector3.up * Stats.JumpStrength, ForceMode.VelocityChange);
            JumpQueued = true;
        }

        private void StartDash(InputAction.CallbackContext ctx)
        {
            RB.AddExplosionForce(50, this.transform.position + this.transform.right, 10, 0, ForceMode.VelocityChange);
        }

        private void ApplyGravity(in float deltaTime)
        {
            if (!IsGrounded)
            {
                if (LocalMoveDir.y > 0)
                {
                    LocalMoveDir.y -= (Stats.GravityAcceleration * Stats.JumpGravityMulti) * deltaTime;
                }
                else
                {
                    LocalMoveDir.y -= (Stats.GravityAcceleration * Stats.FallGravityMulti) * deltaTime;
                }
            }
            else
            {
                LocalMoveDir.y = 0;
            }
        }

        Ray GroundRay = new Ray();
        RaycastHit GroundHit;
        private void SetGrounded(in float deltaTime)
        {
            GroundRay.origin = HeadTrans.position;
            GroundRay.direction = Vector3.down;

            // if (Physics.Raycast(GroundRay, out RaycastHit hit, Mathf.Abs(RB.velocity.y) + Stats.HeadHeight, Stats.GroundedMask))
            if (Physics.Raycast(GroundRay, out GroundHit, Stats.HeadHeight + 0.1f, Stats.GroundedMask, QueryTriggerInteraction.Ignore))
            {
                IsGrounded = true;
            }
            else
            {
                IsGrounded = false;
            }
        }

        public override void OnFrameUpdate(in float deltaTime)
        {
            ApplyRotation(deltaTime);
        }

        public override void OnPhysicsUpdate(in float deltaTime)
        {
            SetGrounded(deltaTime);
            ApplyMovementInput(deltaTime);
            ApplyGravity(deltaTime);

            Move(deltaTime);
        }

        private void ApplyRotation(in float deltaTime)
        {
            CurrentRotation += InputRotation;
            CurrentRotation.x = Mathf.Clamp(CurrentRotation.x, -Stats.VertLookClamp, Stats.VertLookClamp);
            BodyTrans.localRotation = Quaternion.Euler(0, CurrentRotation.y, 0);
            HeadTrans.localRotation = Quaternion.Euler(CurrentRotation.x, 0, 0);
        }


        private void ApplyMovementInput(in float deltaTime)
        {
            LocalMoveDir.x = MoveInput.x * Stats.MoveSpeed;
            LocalMoveDir.z = MoveInput.z * Stats.MoveSpeed;
        }

        private void Move(in float deltaTime)
        {
            RB.AddForce(BodyTrans.rotation * LocalMoveDir * deltaTime, ForceMode.VelocityChange);
            //RB.MovePosition(RB.position  + (BodyTrans.rotation * LocalMoveDir * Time.deltaTime));
        }
        #region Networked Movement

        protected override void ConstructInputData(out InputData input)
        {
            input = new InputData(InputDirection, CombinedInputRotation, JumpQueued);
            CombinedInputRotation = Vector2.zero;
            JumpQueued = false;
        }
        protected override void ApplyInputData(InputData input)
        {
            //InputDirection = input.MoveInput;
            MoveInput = input.MoveInput;
            InputRotation = input.RotInput;
            if (input.JumpDown && IsGrounded)
            {
                RB.AddForce(Vector3.up * Stats.JumpStrength, ForceMode.VelocityChange);
                //IsGrounded = false;
            }

            if (!base.IsOwner)
            {
                OnFrameUpdate((float)TimeManager.TickDelta);
            }

            OnPhysicsUpdate((float)TimeManager.TickDelta);
        }

        protected override void GetStateData(out StateData state)
        {
            state = new StateData()
            {
                Position = RB.position,
                Rotation = new Vector3(HeadTrans.eulerAngles.x, BodyTrans.eulerAngles.y, 0),
                Velocity = RB.velocity,
            };
        }

        protected override void ApplyStateData(StateData state)
        {
            RB.position = state.Position;
            RB.velocity = state.Velocity;
        }

        #endregion
    }
}
