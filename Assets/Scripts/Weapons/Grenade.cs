using Game.Actors;
using System.Collections.Generic;
using UnityEngine;
using Game.Interfaces;
using FishNet.Object;
using FishNet.Component.Ownership;
using Game.DataStructures.Grenades;

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

        private List<GrenadeComponent> Components = new List<GrenadeComponent>();

        private GrenadeSpawner GrenadeSpawner;

        private List<IGCompOnDetonate> DetonateComponents = new List<IGCompOnDetonate>();
        private List<IGCompOnLaunch> LaunchComponents = new List<IGCompOnLaunch>();
        private List<IGCompForceModifier> ForceModiferComponents = new List<IGCompForceModifier>();
        private List<IGCompOnUpdate> FrameUpdateComponents = new List<IGCompOnUpdate>();
        private List<IGCompOnUpdate> PhysicsUpdateComponents = new List<IGCompOnUpdate>();


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

            if (component is IGCompOnDetonate) DetonateComponents.Add((IGCompOnDetonate)component);
            if (component is IGCompOnLaunch) LaunchComponents.Add((IGCompOnLaunch)component);
            if (component is IGCompForceModifier) ForceModiferComponents.Add((IGCompForceModifier)component);
            if (component is IGCompOnUpdate)
            {
                IGCompOnUpdate comp = (IGCompOnUpdate)component;
                switch (comp.Category)
                {
                    case IGCompOnUpdate.UpdateCategory.Frame: FrameUpdateComponents.Add(comp); break;
                    case IGCompOnUpdate.UpdateCategory.Physics: PhysicsUpdateComponents.Add(comp); break;
                    default:
                        break;
                }
            }

            component.Setup(this);
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
            foreach (var item in DetonateComponents)
            {
                item.OnDetonate();
            }
            OnDetonate();
        }

        private void OnDetonate()
        {
            GrenadeSpawner.DespawnGrenade(this);
        }

        private void Launch()
        {
            foreach (var item in LaunchComponents)
            {
                item.OnLaunch();
            }
        }

        [ObserversRpc]
        public void ConstructAndLaunch(GrenadeComponentList compList, Actor owner, float launchForce, float passedTime = 0)
        {
            ActorOwner = owner;
            this.LaunchForce = launchForce;
            CatchupTime = passedTime;
            foreach (var item in compList.Components)
            {
                if (!GrenadeSpawner.GetComponent(item, out GrenadeComponent comp))
                {
                    Debug.LogError($"Failed to load component {item}");
                    continue;
                }
                this.AddComponent(comp);
            }
            this.Launch();
        }

        private void Update()
        {
            if (!this.IsServerInitialized) return;

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

            foreach (var item in FrameUpdateComponents)
            {
                item.OnUpdate(Time.deltaTime + catchup_delta);
            }
        }

        private void FixedUpdate()
        {
            if (!this.IsServerInitialized) return; 

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

            foreach (var item in PhysicsUpdateComponents)
            {
                item.OnUpdate(Time.fixedDeltaTime + catchup_delta);
            }

            LifeTimer += Time.fixedDeltaTime;
            if (LifeTimer > LifeSpan)
            {
                GrenadeSpawner.DespawnGrenade(this);
            }
        }

        public void AddForce(Vector3 force, ForceMode mode)
        {
            foreach (var item in ForceModiferComponents) { item.ModifyDirectForce(ref force, mode); }
            RB.AddForce(force, mode);
        }

        public void AddExplosionForce(float force, Vector3 origin, float radius, ForceMode mode)
        {
            foreach (var item in ForceModiferComponents) { item.ModifyExplosionForce(ref force, origin, radius, mode); }
            RB.AddExplosionForce(force, origin, radius, 0, mode);
        }
    }
}
