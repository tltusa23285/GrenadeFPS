using UnityEngine;

namespace Game.Weapons.Grenades
{
    public class TimedDetonation : GrenadeComponent, IGCompOnLaunch
    {
        [SerializeField] private float Time;

        private void DetonateAfterTime()
        {
            Root.Detonate();
        }

        void IGCompOnLaunch.OnLaunch()
        {
            Invoke(nameof(DetonateAfterTime), Time);
        }
    }
}
