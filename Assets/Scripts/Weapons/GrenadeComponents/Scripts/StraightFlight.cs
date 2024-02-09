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
            rb.isKinematic = true;
            rb.useGravity = false;

            DefaultInterpolation = rb.interpolation;

            trans = rb.transform;
            Speed = Root.LaunchForce;

            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        protected override void OnSetDown()
        {
            rb.interpolation = DefaultInterpolation;
        }

        Ray TravelRay;
        float TravelDist;
        RaycastHit Hit;
        public override void OnPhysicsUpdate(in float time)
        {
            TravelRay.origin = rb.position;
            TravelRay.direction = trans.forward;
            TravelDist = Speed * time;

            if (Physics.Raycast(TravelRay, out Hit, TravelDist))
            {
                Speed = 0;
                rb.MovePosition(Hit.point);
            }
            else
            {
                rb.MovePosition(rb.position + (trans.forward * TravelDist));
            }
        }
    }
}
