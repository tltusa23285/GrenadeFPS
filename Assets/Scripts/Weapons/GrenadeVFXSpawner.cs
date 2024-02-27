using Game.DataStructures.Grenades;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Weapons.Grenades
{
    public class GrenadeVFXSpawner : MonoBehaviour
    {
        private Dictionary<string, GameObject> Sources = new Dictionary<string, GameObject>();
        private Dictionary<GameObject, Queue<GameObject>> Pools = new Dictionary<GameObject, Queue<GameObject>>();

        public GrenadeComponentMap AvailableAssets;

        private void Awake()
        {
            //InstanceFinder.RegisterInstance(this);
        }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            Sources.Clear();
            foreach (var item in Pools)
            {
                foreach (var obj in item.Value)
                {
                    Destroy(obj);
                }
            }
            Pools.Clear();

            foreach (var item in AvailableAssets.AvailableComponents)
            {
                GameObject source = Addressables.LoadAssetAsync<GameObject>(item.AddressableName).WaitForCompletion();
                Sources.Add(item.Identifier, source);
                Pools.Add(source, new Queue<GameObject>());
            }
        }

        private bool GetAsset(in string id, out GameObject asset)
        {
            asset = null;
            if (!Sources.ContainsKey(id)) return false;
            if (!Pools.ContainsKey(Sources[id])) return false;
            if (!Pools[Sources[id]].TryDequeue(out asset))
            {
                asset = Instantiate(Sources[id]);
            }

            return asset != null;
        }

        private void ReturnAsset(in string id, GameObject asset)
        {
            if (!Pools.ContainsKey(Sources[id]))
            {
                Destroy(asset);
            }
            else
            {
                Pools[Sources[id]].Enqueue(asset);
            }
        }

        public bool SpawnVfx(in string id, in Vector3 position, in Quaternion rotation, out GameObject vfx)
        {
            vfx = null;

            //GetAsset(id, out vfx);

            //vfx.transform.position = position;
            //vfx.transform.rotation = rotation;

            return vfx != null;
        }

        public void ReturnVFX(GameObject obj)
        {
            Destroy(obj);
        }
    }
}
