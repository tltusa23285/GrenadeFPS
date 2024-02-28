using Game.DataStructures.Grenades;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using FishNet;

namespace Game.Weapons.Grenades
{
    public class GrenadeVFXSpawner : MonoBehaviour
    {
        private Dictionary<string, GameObject> Sources = new Dictionary<string, GameObject>();
        private Dictionary<string, Queue<GameObject>> InactivePools = new Dictionary<string, Queue<GameObject>>();

        public GrenadeComponentMap AvailableAssets;

        private void Awake()
        {
            InstanceFinder.RegisterInstance(this);
        }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            Sources.Clear();
            foreach (var item in InactivePools)
            {
                foreach (var obj in item.Value)
                {
                    Destroy(obj);
                }
            }
            InactivePools.Clear();

            foreach (var item in AvailableAssets.AvailableComponents)
            {
                GameObject source = Addressables.LoadAssetAsync<GameObject>(item.AddressableName).WaitForCompletion();
                Sources.Add(item.Identifier, source);
                InactivePools.Add(item.Identifier, new Queue<GameObject>());
            }
        }

        private bool GetAsset(in string id, out GameObject asset)
        {
            asset = null;
            if (!Sources.ContainsKey(id)) return false;
            if (!InactivePools.ContainsKey(id)) return false;
            if (!InactivePools[id].TryDequeue(out asset))
            {
                asset = Instantiate(Sources[id]);
            }

            return asset != null;
        }

        private void ReturnAsset(in string id, GameObject asset)
        {
            if (!InactivePools.ContainsKey(id))
            {
                Destroy(asset);
            }
            else
            {
                InactivePools[id].Enqueue(asset);
            }
        }

        public bool SpawnVfx(in string id, in Vector3 position, in Quaternion rotation, in float lifetime, out GameObject vfx)
        {
            vfx = null;

            if(!GetAsset(id, out vfx)) return false;

            vfx.transform.position = position;
            vfx.transform.rotation = rotation;

            if (vfx != null)
            {
                StartCoroutine(ReturnAfterTime(lifetime, id, vfx));
                return true;
            }
            return false;
        }

        IEnumerator ReturnAfterTime(float lifetime, string id, GameObject asset)
        {
            yield return new WaitForSeconds(lifetime);
            ReturnAsset(id, asset);
        }
    }
}
