using UnityEngine;
using Game.Interfaces;

namespace Game.Weapons.Grenades
{
    public class Explosive : GrenadeComponent, IGCompOnDetonate
    {
        [SerializeField] private int Damage = 50;
        [SerializeField] private float PushForce = 20;
        [SerializeField] private float Radius = 10;

        void IGCompOnDetonate.OnDetonate()
        {
            foreach (var item in Physics.OverlapSphere(Root.transform.position, Radius))
            {
                if (item.TryGetComponent(out IDamageable dmg))
                {
                    dmg.TakeDamage(Damage);
                }
                if (item.TryGetComponent(out IForceable rb))
                {
                    rb.AddExplosionForce(PushForce, Root.transform.position, Radius, ForceMode.VelocityChange);
                }
            }
        }
    }
}
