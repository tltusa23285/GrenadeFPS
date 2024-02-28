using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapons.Grenades
{
    public class TimedDetonation : GrenadeComponent
    {
        [SerializeField] private float Time;
        protected override void OnSetup()
        {
            StartCoroutine(DetonateAfterTime());
        }

        IEnumerator DetonateAfterTime()
        {
            yield return new WaitForSeconds(Time);
            Root.Detonate();
        }
    }
}
