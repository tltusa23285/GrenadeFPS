using UnityEngine;

namespace Game.Weapons.Grenades
{
    public class Lob : GrenadeComponent, IGCompOnLaunch
    {
        protected override void OnSetup()
        {
            Root.RB.useGravity = true;
        }

        protected override void OnSetDown()
        {
            Root.RB.velocity = Vector3.zero;
            Root.RB.angularVelocity = Vector3.zero;
        }

        void IGCompOnLaunch.OnLaunch()
        {
            Root.RB.AddForce(Root.Trans.forward * Root.LaunchForce, ForceMode.VelocityChange);
        }
    }
}
