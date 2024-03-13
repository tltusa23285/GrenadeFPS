using FishNet.Object;
using Game.Actors;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using Game.DataStructures.Grenades;
using UnityEngine.AddressableAssets;

namespace Game.Weapons.Grenades
{
    /// <summary>
    /// Networked singleton responsible for creation and destruction of grenade objects
    /// </summary>
    public class GrenadeSpawner : NetworkBehaviour
    {

        private Dictionary<string, GameObject> ComponentSources = new Dictionary<string, GameObject>();
        private Dictionary<string, Queue<GrenadeComponent>> ComponentPool = new Dictionary<string, Queue<GrenadeComponent>>();
        private Queue<Grenade> GrenadePool = new Queue<Grenade>();

        public GrenadeComponentMap Components;

        private void Awake()
        {
            InstanceFinder.RegisterInstance<GrenadeSpawner>(this);
        }

        public GameObject GrenadePrefab;

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            ComponentSources.Clear();
            foreach (var item in ComponentPool)
            {
                foreach (var comp in item.Value)
                {
                    Destroy(comp.gameObject);
                }
            }
            ComponentPool.Clear();

            foreach (var item in Components.AvailableComponents)
            {
                ComponentSources.Add(item.Identifier, Addressables.LoadAssetAsync<GameObject>(item.AddressableName).WaitForCompletion());
                ComponentPool.Add(item.Identifier, new Queue<GrenadeComponent>());
            }
        }
        
        public bool GetComponent(in string id, out GrenadeComponent comp)
        {
            comp = null;
            if (!ComponentSources.ContainsKey(id))
            {
                Debug.LogError($"Attempted to get component with ID {id} when it was not in the available list");
                return false;
            }
            if (!ComponentPool.ContainsKey(id))
            {
                Debug.LogWarning($"Attempted to get component with ID {id} when it did not have an established pool, generating now");
                ComponentPool.Add(id, new Queue<GrenadeComponent>());
            }

            if (!ComponentPool[id].TryDequeue(out comp))
            {
                GameObject go = Instantiate(ComponentSources[id], new Vector3(0, 5000, 0), Quaternion.identity);
                if (!go.TryGetComponent(out comp))
                {
                    Debug.LogError($"Loaded prefab for component {id} did not contain a component script");
                    return false;
                }
                comp.PoolID = id;
            }

            comp.transform.parent = null;

            return true;
        }

        private void ReturnComponent(GrenadeComponent comp)
        {
            if (!ComponentPool.ContainsKey(comp.PoolID))
            {
                Debug.LogError($"Tried to return component with pool id {comp.PoolID} when pool did not exist, destroying component");
                Destroy(comp);
                return;
            }

            ComponentPool[comp.PoolID].Enqueue(comp);

            comp.transform.parent = this.transform;
        }

        //private void GetGrenade(out Grenade gre)
        //{
        //    if (!GrenadePool.TryDequeue(out gre))
        //    {
        //        GameObject go = Instantiate(GrenadePrefab, new Vector3(0, 10000, 0), Quaternion.identity);
        //        gre = go.GetComponent<Grenade>();
        //    }
        //    gre.gameObject.SetActive(true);
        //    gre.transform.parent = null;
        //}

        //private void ReturnGrenade(Grenade gre)
        //{
        //    foreach (var item in gre.Components)
        //    {
        //        ReturnComponent(item);
        //    }
        //    gre.FlushComponents();
        //    GrenadePool.Enqueue(gre);
        //    gre.gameObject.SetActive(false);
        //    gre.transform.parent = this.transform;
        //}

        [ServerRpc(RequireOwnership = false)]
        public void SpawnGrenade(GrenadeComponentList compList, Vector3 position, Quaternion rotation, Actor owner, float launchForce, float passedTime = 0f)
        {
            GameObject go = Instantiate(GrenadePrefab, position, rotation);
            Grenade g = go.GetComponent<Grenade>();

            //g.transform.position = position;
            //g.transform.rotation = rotation;

            //foreach (var item in compList.Components)
            //{
            //    if (!GetComponent(item, out GrenadeComponent comp))
            //    {
            //        Debug.LogError($"Failed to load component {item}");
            //        continue;
            //    }
            //    g.AddComponent(comp);
            //}

            InstanceFinder.ServerManager.Spawn(go, owner.Owner);

            g.ConstructAndLaunch(compList, owner, launchForce, passedTime);

            //return g;
        }

        public void DespawnGrenade(Grenade g)
        {
            g.Despawn();
        }
    }
}
