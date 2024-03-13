using Game.DataStructures.Grenades;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using FishNet;
using Game.VfxScripts;

namespace Game.Weapons.Grenades
{
    public class GrenadeVFXSpawner : MonoBehaviour
    {
        private Dictionary<string, GameObject> Sources = new Dictionary<string, GameObject>();
        private Dictionary<string, Queue<VfxObject>> Pool = new Dictionary<string, Queue<VfxObject>>();

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
            foreach (var item in Pool)
            {
                foreach (var obj in item.Value)
                {
                    obj.Dispose();
                }
            }
            Pool.Clear();

            foreach (var item in AvailableAssets.AvailableComponents)
            {
                GameObject source = Addressables.LoadAssetAsync<GameObject>(item.AddressableName).WaitForCompletion();
                if (!source.TryGetComponent(out VfxObject ve))
                {
                    Debug.LogError($"Source of id {item.Identifier} does not implement IVisualEffect");
                    continue;
                }
                Sources.Add(item.Identifier, source);
                Pool.Add(item.Identifier, new Queue<VfxObject>());
            }
        }

        private bool GetAsset(in string id, out VfxObject asset)
        {
            asset = null;
            if (!Sources.ContainsKey(id)) return false;
            if (!Pool.ContainsKey(id)) return false;
            if (!Pool[id].TryDequeue(out asset))
            {
                GameObject go = Instantiate(Sources[id]);
                asset = go.GetComponent<VfxObject>();
            }

            return asset != null;
        }

        private void ReturnAsset(in string id, VfxObject asset)
        {
            if (!Pool.ContainsKey(id))
            {
                asset.Dispose();
            }
            else
            {
                asset.Stop();
                Pool[id].Enqueue(asset);
            }
        }

        public bool SpawnVfx(in string id, in Vector3 position, in Quaternion rotation, in float lifetime, out VfxObject vfx)
        {
            vfx = null;

            if(!GetAsset(id, out vfx)) return false;

            vfx.transform.position = position;
            vfx.transform.rotation = rotation;

            if (vfx != null)
            {
                vfx.Duration = lifetime;
                StartCoroutine(ReturnAfterTime(lifetime, id, vfx));
                return true;
            }
            return false;
        }

        IEnumerator ReturnAfterTime(float lifetime, string id, VfxObject asset)
        {
            yield return new WaitForSeconds(lifetime);
            ReturnAsset(id, asset);
        }
    }
}
