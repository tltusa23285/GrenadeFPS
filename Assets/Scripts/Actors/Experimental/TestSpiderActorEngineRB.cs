using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Actors.Components
{
    [RequireComponent(typeof(Rigidbody))]
    public class TestSpiderActorEngineRB : ActorEngine
    {

        private Rigidbody Body;

        public InputAction DashAction;

        protected Vector3 GravityDirection 
        {
            get;set;
        }

        protected override void OnAwake()
        {
            Body = GetComponent<Rigidbody>();
            //Body.useGravity = false;

            Body.freezeRotation = true;

            DashAction.performed += ctx => { StartJump(ctx); };
            GravityDirection = Vector3.down;
        }

        private void OnEnable()
        {
            DashAction.Enable();
        }

        private void OnDisable()
        {
            DashAction.Disable();
        }

        void StartJump(InputAction.CallbackContext ctx)
        {

        }

        Vector3 UpDirection = Vector3.up;
        Vector3 CameraForward;
        private void FixedUpdate()
        {
            //SetUpDirection(out RaycastHit hit);

            //BodyTrans.rotation = Quaternion.FromToRotation(BodyTrans.up, UpDirection) * BodyTrans.rotation;

            //HeadTrans.position = BodyTrans.position + (UpDirection * Stats.HeadHeight);
            ////HeadTrans.rotation = Quaternion.FromToRotation(HeadTrans.up, up) * HeadTrans.rotation;
            //HeadTrans.rotation = BodyTrans.rotation;

            //Camera.transform.localRotation = Quaternion.Euler(InputRotation.x, InputRotation.y, 0);

            //CameraForward = Camera.transform.forward;
            //CameraForward = HeadTrans.InverseTransformDirection(CameraForward);
            //CameraForward = HeadTrans.TransformDirection(CameraForward.FlattenY().normalized);

            //Vector3 dir = ((InputDirection.z * CameraForward) + (InputDirection.x * Camera.transform.right));

            //float accel = (dir * Stats.MoveSpeed).magnitude - Body.velocity.magnitude;

            //Body.AddForce(dir, ForceMode.VelocityChange);
        }

        private void SetUpDirection(out RaycastHit hit)
        {
            hit = default;
            //if (Physics.Raycast(Body.position + (Stats.HeadHeight * UpDirection), 
            //                    -UpDirection, 
            //    out hit, Stats.HeadHeight + 0.1f, Stats.GroundedMask))
            //{
            //    UpDirection = hit.normal;
            //}
            //else { UpDirection = Vector3.up; }

            //if (hit.distance > 0.5f)
            //{
            //    Body.AddForce(-UpDirection * 9.8f, ForceMode.Acceleration);
            //}
        }

        private Vector3 DampVelocityCache;
        private float DampAngleCache;
        private float DampDelta;
        private float DampLerp;

        private void LateUpdate()
        {
            //CameraTrans.position = Vector3.SmoothDamp(CameraTrans.position, HeadTrans.position, ref DampVelocityCache, Time.fixedDeltaTime);

            //DampDelta = Quaternion.Angle(CameraTrans.rotation, HeadTrans.rotation);
            //if (DampDelta > 0)
            //{
            //    DampLerp = Mathf.SmoothDampAngle(DampDelta, 0, ref DampAngleCache, Time.fixedDeltaTime);
            //    DampLerp = 1.0f - DampLerp / DampDelta;
            //    CameraTrans.rotation = Quaternion.Slerp(CameraTrans.rotation, HeadTrans.rotation, DampLerp);
            //}            
        }
    }
}
