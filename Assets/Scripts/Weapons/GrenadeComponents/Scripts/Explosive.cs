using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Interfaces;

namespace Game.Weapons.Grenades
{
    public class Explosive : GrenadeComponent
    {
        [SerializeField] private int Damage = 50;
        [SerializeField] private float PushForce = 20;
        [SerializeField] private float Radius = 10;
        protected override void OnSetup()
        {
        }

        protected override void OnSetDown()
        {
        }

        protected override void OnVisualDetonate()
        {
            Debug.DrawRay(Root.transform.position, Vector3.up * Radius,         Color.red , 3f);
            Debug.DrawRay(Root.transform.position, Vector3.down * Radius,       Color.red , 3f);
            Debug.DrawRay(Root.transform.position, Vector3.left * Radius,       Color.red , 3f);
            Debug.DrawRay(Root.transform.position, Vector3.right * Radius,      Color.red , 3f);
            Debug.DrawRay(Root.transform.position, Vector3.forward * Radius,    Color.red , 3f);
            Debug.DrawRay(Root.transform.position, Vector3.back * Radius,       Color.red , 3f);
        }

        protected override void OnFunctionalDetonate()
        {
            foreach (var item in Physics.OverlapSphere(Root.transform.position, Radius))
            {
                if (item.TryGetComponent(out IDamageable dmg))
                {
                    dmg.TakeDamage(Damage);
                }
                if (item.TryGetComponent(out Rigidbody rb))
                {
                    rb.AddExplosionForce(PushForce, Root.transform.position, Radius, 0, ForceMode.VelocityChange);
                }
            }
        }
    }
}
