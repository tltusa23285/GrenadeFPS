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
        public string[] Components;
        public GrenadeComponentList(params string[] comps)
        {
            Components = comps;
        }
    }

    [System.Serializable]
    public struct GrenadeComponentData
    {
        public string DisplayName;
        public string Identifier;
        public string Description;

        // TODO : currently unsure if we want these to be separate in anyway, revist after more exploration of addressables
        public string AddressableName => Identifier;
    }
}
