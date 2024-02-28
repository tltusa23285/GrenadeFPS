using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Interfaces;
using FishNet.Object;
using FishNet;
using UnityEngine.VFX;
using Game.VfxScripts;

namespace Game.Weapons.Grenades
{
    public class Implosive : GrenadeComponent
    {
        [SerializeField] private int Damage = 50;
        [SerializeField] private float PullForce = 20;
        [SerializeField] private float Radius = 10;
        protected override void OnVisualDetonate()
        {
            if (InstanceFinder.GetInstance<GrenadeVFXSpawner>().SpawnVfx("ImplosiveVFX", this.transform.position, this.transform.rotation, 0.25f, out VfxObject go))
            {
                if (go.TryGetComponent(out VisualEffect vfx))
                {
                    vfx.SetFloat("Radius", Radius);
                }
                go.Play();
            }
        }

        protected override void OnFunctionalDetonate()
        {
            foreach (var item in Physics.OverlapSphere(Root.transform.position, Radius))
            {
                if (item.TryGetComponent(out IDamageable dmg))
                {
                    dmg.TakeDamage(Damage);
                }
                if (item.TryGetComponent(out IForceable rb))
                {
                    rb.AddExplosionForce(-PullForce, Root.transform.position, Radius, ForceMode.VelocityChange);
                }
            }
        }
    }
}
