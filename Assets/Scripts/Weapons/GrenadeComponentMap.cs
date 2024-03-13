using UnityEngine;

namespace Game.DataStructures.Grenades
{
    [System.Serializable]
    public struct GrenadeComponentMap
    {
        public GrenadeComponentData[] AvailableComponents;
    }

    [System.Serializable]
    public struct GrenadeComponentList
    {
        [AddressableSelector] public string[] Components;
        public GrenadeComponentList(params string[] comps)
        {
            Components = comps;
        }
    }

    [System.Serializable]
    public struct GrenadeComponentData
    {
        [AddressableSelector] public string Identifier;
        public string DisplayName;
        [TextArea(2,4)]public string Description;

        // TODO : currently unsure if we want these to be separate in anyway, revist after more exploration of addressables
        public string AddressableName => Identifier;
    }
}
