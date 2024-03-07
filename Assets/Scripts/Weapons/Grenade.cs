using Game.Actors;
using System.Collections.Generic;
using UnityEngine;
using Game.Interfaces;
using FishNet.Object;
using FishNet.Component.Prediction;
using FishNet.Component.Ownership;

namespace Game.Weapons.Grenades
{
    public class Grenade : PredictedSpawn, IForceable
    {
        public Actor ActorOwner { get; private set; }

        [Header("Grenades")]

        public float LifeSpan = 10f;
        private float LifeTimer = 0;

        public float LaunchForce { get; private set; }
        public float CatchupTime { get; private set; }
        public Rigidbody RB {get; private set; }
        public Transform Trans { get; private set; }
        public SphereCollider Collider { get; private set; }

        public List<GrenadeComponent> Components = new List<GrenadeComponent>();

        private GrenadeSpawner GrenadeSpawner;

        private void Awake()
        {
            GrenadeSpawner = FishNet.InstanceFinder.GetInstance<GrenadeSpawner>();

            Trans = GetComponent<Transform>();
            RB = GetComponent<Rigidbody>();
            Collider = GetComponent<SphereCollider>();
        }

        public void AddComponent(GrenadeComponent component)
        {
            Components.Add(component);
        }

        public void FlushComponents()
        {
            foreach (var item in Components)
            {
                item.SetDown();
            }

            Components.Clear();
        }

        private void OnEnable()
        {
            LifeTimer = 0;
            RB.position = this.transform.position;
            RB.rotation = this.transform.rotation;
        }

        private void OnDisable()
        {
            RB.interpolation = RigidbodyInterpolation.None;
        }

        public void Detonate()
        {
            foreach (var item in Components)
            {
                item.Detonate();
            }
            OnDetonate();
        }

        private void OnDetonate()
        {
            GrenadeSpawner.DespawnGrenade(this);
        }

        public void Launch(Actor owner, float LaunchForce, float passedTime)
        {
            ActorOwner = owner;
            this.LaunchForce = LaunchForce;
            CatchupTime = passedTime;

            foreach (var item in Components)
            {
                item.Setup(this);
            }
        }


        private void Update()
        {
            float delta = Time.deltaTime;
            float catchup_delta = 0;
            if (CatchupTime > 0f)
            {
                float step = (CatchupTime * 0.08f);
                CatchupTime -= step;
                if (CatchupTime <= delta/2f)
                {
                    step += CatchupTime;
                    CatchupTime = 0f;
                }
                catchup_delta = step;
            }

            foreach (var item in Components)
            {
                item.OnFrameUpdate(Time.deltaTime + catchup_delta);
            }
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;
            float catchup_delta = 0;
            if (CatchupTime > 0f)
            {
                float step = (CatchupTime * 0.08f);
                if (CatchupTime <= delta / 2f)
                {
                    step += CatchupTime;
                }
                catchup_delta = step;
            }

            foreach (var item in Components)
            {
                item.OnPhysicsUpdate(Time.fixedDeltaTime + catchup_delta);
            }

            LifeTimer += Time.fixedDeltaTime;
            if (LifeTimer > LifeSpan)
            {
                GrenadeSpawner.DespawnGrenade(this);
            }
        }

        public void AddForce(Vector3 force, ForceMode mode)
        {
            foreach (var item in Components) { item.ModifyDirectForce(ref force, mode); }
            RB.AddForce(force, mode);
        }

        public void AddExplosionForce(float force, Vector3 origin, float radius, ForceMode mode)
        {
            foreach (var item in Components) { item.ModifyExplosionForce(ref force, origin, radius, mode); }
            RB.AddExplosionForce(force, origin, radius, 0, mode);
        }
    }
}
