using Game.Actors;
using UnityEngine;
using UnityEngine.Assertions;
using FishNet.Object;
using Game.DataStructures.Grenades;
using Game.Weapons.Grenades;

namespace Game.Weapons
{
    public class Launcher : NetworkBehaviour
    {
        private const float MAX_PASSED_TIME = 0.3f; // in seconds

        public Transform Firepoint;
        public float LaunchForce = 10;
        public GameObject GrenadePrefab;

        [SerializeField, Tooltip("Measured in Shots per Second")]
        protected float _fireRate;
        public virtual float FireRate
        {
            get { return _fireRate; }
            set 
            { 
                _fireRate = value;
                ShotCooldown = 1 / _fireRate;
            }
        }
        protected float ShotCooldown;
        protected float ShotTimer;

        private Actor ActorOwner;
        private GrenadeSpawner GrenadeSpawner;

        public GrenadeComponentList UsedComponents;

        private void Awake()
        {
            Assert.IsNotNull(Firepoint);
            Assert.IsNotNull(GrenadePrefab);
            GrenadeSpawner = FishNet.InstanceFinder.GetInstance<GrenadeSpawner>();
            OnAwake();
        }

        public void Initialize(Actor owner)
        {
            ActorOwner = owner;
        }

        protected virtual void OnAwake()
        {
            FireRate = FireRate;
            ShotTimer = float.MaxValue;
        }

        protected virtual void Update()
        {
            if (ShotTimer < ShotCooldown)
            {
                ShotTimer += Time.deltaTime;
            }
        }

        public virtual void Fire()
        {
            if (ShotTimer < ShotCooldown) return;
            ShotTimer = 0;

            if(!IsHostInitialized) GrenadeSpawner.SpawnGrenadeLocal(UsedComponents, Firepoint.position, Firepoint.rotation, ActorOwner, LaunchForce);
            FireServer(UsedComponents, Firepoint.position, Firepoint.rotation, ActorOwner, LaunchForce, base.TimeManager.Tick);
        }

        [ServerRpc]
        private void FireServer(GrenadeComponentList compList, Vector3 position, Quaternion rotation, Actor owner, float launchForce, uint tick)
        {
            float passed_time = (float)base.TimeManager.TimePassed(tick, false);
            passed_time = Mathf.Min(MAX_PASSED_TIME / 2f, passed_time);

            GrenadeSpawner.SpawnGrenadeLocal(compList, position, rotation, owner, launchForce, passed_time);

            FireObserver(compList, position, rotation, owner, launchForce, tick);
        }

        [ObserversRpc(ExcludeOwner = true)]
        private void FireObserver(GrenadeComponentList compList, Vector3 position, Quaternion rotation, Actor owner, float launchForce, uint tick)
        {
            float passed_time = (float)base.TimeManager.TimePassed(tick, false);
            passed_time = Mathf.Min(MAX_PASSED_TIME / 2f, passed_time);

            if(!IsHostInitialized) GrenadeSpawner.SpawnGrenadeLocal(compList, position, rotation, owner, launchForce, passed_time);
        }
    }
}
