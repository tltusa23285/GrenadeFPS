using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapons.Grenades
{
    public class Lob : GrenadeComponent
    {
        protected override void OnSetup()
        {
            Root.RB.useGravity = true;
            Root.RB.AddForce(Root.Trans.forward * Root.LaunchForce, ForceMode.VelocityChange);
        }

        protected override void OnSetDown()
        {
            Root.RB.velocity = Vector3.zero;
            Root.RB.angularVelocity = Vector3.zero;
        }
    }
}
