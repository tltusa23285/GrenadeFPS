using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapons.Grenades
{
    public class ContactDetonation : GrenadeComponent
    {
        protected override void OnSetup()
        {
        }

        protected override void OnSetDown()
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            Root.Detonate();
        }
    }
}
