using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapons.Grenades
{
    public class StraightFlight : GrenadeComponent, IGCompOnUpdate, IGCompForceModifier
    {
        private Transform trans;
        private Rigidbody rb;
        private float Speed;

        private RigidbodyInterpolation DefaultInterpolation;

        protected override void OnSetup()
        {
            rb = Root.RB;
            // rb.isKinematic = true;
            rb.useGravity = false;

            DefaultInterpolation = rb.interpolation;

            trans = rb.transform;
            Speed = Root.LaunchForce;

            //rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        protected override void OnSetDown()
        {
            rb.interpolation = DefaultInterpolation;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        IGCompOnUpdate.UpdateCategory IGCompOnUpdate.Category => IGCompOnUpdate.UpdateCategory.Physics;

        void IGCompOnUpdate.OnUpdate(in float deltaTime)
        {
            rb.AddForce(trans.forward * Speed, ForceMode.Acceleration);
        }

        void IGCompForceModifier.ModifyDirectForce(ref Vector3 force, in ForceMode mode)
        {
            force /= 2;
        }

        void IGCompForceModifier.ModifyExplosionForce(ref float force, in Vector3 origin, in float radius, ForceMode mode)
        {

            force /= 2;
        }
    }
}
