using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapons.Grenades
{
    public class ContactDetonation : GrenadeComponent
    {
        private void OnTriggerEnter(Collider other)
        {
            Root.Detonate();
        }
    }
}
