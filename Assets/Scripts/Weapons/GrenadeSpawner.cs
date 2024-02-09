using FishNet.Object;
using Game.Actors;
using Game.Weapons.Grenades;
using System.Collections.Generic;
using UnityEngine;
using FishNet;

namespace Game.Weapons
{
    /// <summary>
    /// Networked singleton responsible for creation and destruction of grenade objects
    /// </summary>
    public class GrenadeSpawner : MonoBehaviour
    {

        private Dictionary<GameObject, Queue<GrenadeComponent>> ComponentPool = new Dictionary<GameObject, Queue<GrenadeComponent>>();

        public GameObject[] ComponentPrefabs;

        private void Awake()
        {
            InstanceFinder.RegisterInstance<GrenadeSpawner>(this);
        }

        public GameObject GrenadePrefab;

        private void Start()
        {
            foreach (var item in ComponentPrefabs)
            {
                ComponentPool.Add(item, new Queue<GrenadeComponent>());
            }
        }

        public Grenade SpawnGrenadeLocal(in Vector3 position, in Quaternion rotation, in Actor owner, in float launchForce, in float passedTime = 0f)
        {
            Debug.Log($"Spawning grenade");
            GameObject obj = GameObject.Instantiate(GrenadePrefab, position, rotation);
            Grenade g = obj.GetComponent<Grenade>();

            GrenadeComponent comp = null;

            foreach (var item in ComponentPrefabs)
            {
                comp = Instantiate(item, new Vector3(0, 5000, 0), Quaternion.identity).GetComponent<GrenadeComponent>();
                comp.Prefab = item;
                g.AddComponent(comp);
            }

            g.Launch(owner, launchForce, passedTime);

            return g;
        }

        public void DespawnGrenade(Grenade g)
        {
            g.FlushComponents();
            Destroy(g.gameObject);
        }
    }
}
