using FishNet;
using Game.VfxScripts;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.Weapons.Grenades
{
    public class PayloadVfx : GrenadeComponent, IGCompOnDetonate
    {
        [SerializeField] private string VfxToSpawn;
        [SerializeField] private float Radius = 10;
        void IGCompOnDetonate.OnDetonate()
        {
            if (InstanceFinder.GetInstance<GrenadeVFXSpawner>().SpawnVfx(VfxToSpawn, this.transform.position, this.transform.rotation, 0.25f, out VfxObject go))
            {
                if (go.TryGetComponent(out VisualEffect vfx))
                {
                    vfx.SetFloat("hydrthydt", 5);
                    vfx.SetFloat("Radius", Radius);
                }
                go.Play();
            }
        }
    }
}
