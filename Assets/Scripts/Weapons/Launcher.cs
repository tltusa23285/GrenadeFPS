using Game.Actors;
using UnityEngine;
using UnityEngine.Assertions;
using FishNet.Object;
using Game.DataStructures.Grenades;
using Game.Weapons.Grenades;

namespace Game.Weapons
{
    public class Launcher : MonoBehaviour
    {
        public Transform Firepoint;
        public float LaunchForce = 10;

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

            GrenadeSpawner.SpawnGrenade(UsedComponents, Firepoint.position, Firepoint.rotation, ActorOwner, LaunchForce);
        }
    }
}
