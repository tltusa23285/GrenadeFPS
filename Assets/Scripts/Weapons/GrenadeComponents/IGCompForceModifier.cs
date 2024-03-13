using UnityEngine;

namespace Game.Weapons.Grenades
{
    public interface IGCompForceModifier
    {
        public void ModifyDirectForce(ref Vector3 force, in ForceMode mode);
        public void ModifyExplosionForce(ref float force, in Vector3 origin, in float radius, ForceMode mode);
    }
}
