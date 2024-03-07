using UnityEngine;

namespace Game.Weapons.Grenades
{
    public abstract class GrenadeComponent : MonoBehaviour
    {
        protected Grenade Root;

        [HideInInspector] public string PoolID;

        /// <summary>
        /// Initializes the component, completely resetting its state
        /// </summary>
        /// <param name="root"></param>
        public void Setup(Grenade root)
        {
            Root = root;

            // ensure our collider does not interact with any other collider on the grenade
            // TODO : if a single component contains multiple colliders, reevaluate to decide if components should handle this individually, and/or move this check as base of OnSetup
            if (this.TryGetComponent(out Collider coll))
            {
                foreach (var item in root.gameObject.GetComponentsInChildren<Collider>())
                {
                    Physics.IgnoreCollision(item, coll, true);
                }
                foreach (var item in root.ActorOwner.GetComponentsInChildren<Collider>())
                {
                    Physics.IgnoreCollision(item, coll, true);
                }
            }

            this.transform.parent = root.transform;
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
            this.gameObject.SetActive(true);
            OnSetup();
        }
        /// <summary>
        /// Cleans up anything running/created by the comopnent, should be called when it is no longer in use.
        /// </summary>
        public void SetDown()
        {
            OnSetDown();

            // TODO : currently grenade spawner handles parents of objects going in and out of pools
            //this.transform.parent = null; 
            Root = null;
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Invoked when setup/spawned, this should fully reset this component to a fresh state
        /// </summary>
        protected virtual void OnSetup() { }

        /// <summary>
        /// Invoked when setdown/despawned, this should halt any looping behaviour, and clean up any lingering objcts needed for function
        /// </summary>
        protected virtual void OnSetDown() { }

        /// <summary>
        /// Update method controlled/called by roots Update
        /// Guarenteed to run only when the root grenade is allowed to run its Update
        /// </summary>
        /// <param name="time"></param>
        public virtual void OnFrameUpdate(in float time) { }

        /// <summary>
        /// Update method controlled/called by roots FixedUpdate
        /// Guarenteed to run only when the root grenade is allowed to run its FixedUpdate
        /// </summary>
        /// <param name="time"></param>
        public virtual void OnPhysicsUpdate(in float time) { }

        public void Detonate()
        {
            if (FishNet.InstanceFinder.IsClientStarted) OnVisualDetonate();
            if (FishNet.InstanceFinder.IsServerStarted) OnFunctionalDetonate();
        }

        /// <summary>
        /// Executes any visual behaviour when root is detonated
        /// </summary>
        protected virtual void OnVisualDetonate() { }
        /// <summary>
        /// Executes any functional behaviour when root is detonated
        /// </summary>
        protected virtual void OnFunctionalDetonate() { }

        public virtual void ModifyDirectForce(ref Vector3 force, in ForceMode mode) { }
        public virtual void ModifyExplosionForce(ref float force, in Vector3 origin, in float radius, ForceMode mode) { }
    }
}
