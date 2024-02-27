using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapons.Grenades
{
    public class StraightFlight : GrenadeComponent
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

            rb.AddForce(trans.forward * Speed, ForceMode.VelocityChange);
        }

        protected override void OnSetDown()
        {
            rb.interpolation = DefaultInterpolation;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Ray TravelRay;
        float TravelDist;
        RaycastHit Hit;
        public override void OnPhysicsUpdate(in float time)
        {
            rb.AddForce(trans.forward * Speed, ForceMode.Acceleration);
            //TravelRay.origin = rb.position;
            //TravelRay.direction = trans.forward;
            //TravelDist = Speed * time;

            //if (Physics.Raycast(TravelRay, out Hit, TravelDist))
            //{
            //    Speed = 0;
            //    rb.MovePosition(Hit.point);
            //}
            //else
            //{
            //    rb.MovePosition(rb.position + (trans.forward * TravelDist));
            //}
        }

        public override void ModifyDirectForce(ref Vector3 force, in ForceMode mode)
        {
            force /= 2;
        }

        public override void ModifyExplosionForce(ref float force, in Vector3 origin, in float radius, ForceMode mode)
        {
            force /= 2;
        }
    }
}
