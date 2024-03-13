using Game.Actors.Components;
using Game.Interfaces;
using Game.Weapons;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

namespace Game.Actors
{
    /// <summary>
    /// Routes/translates input to various actor components
    /// </summary>
    [RequireComponent(typeof(ActorEngine))]
	public class Actor : NetworkBehaviour, IDamageable
	{
        public Launcher Launcher;
        
        private ActorEngine Engine;

        private Vector2 MoveInput;
        private Vector2 RotInput;
        private Vector2 CombinedRotInputs;

        #region Monobehaviour
        private void Awake()
        {
            Engine = GetComponent<ActorEngine>();
        }

        void Start()
        {
            Launcher.Initialize(this);
        }

        public void SetupActor(in bool isOwner, in bool isServer, in bool isHost)
        {
            this.Engine.Setup(isOwner, isServer, isHost);
            this.gameObject.name = $"Player-Actor:{this.OwnerId}";
        }

        public void DespawnActor()
        {
            this.Despawn();
        }

        public void Update()
        {
            if (!IsOwner) return;
            MoveInput.x = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;
            MoveInput.y = Input.GetKey(KeyCode.S) ? -1 : Input.GetKey(KeyCode.W) ? 1 : 0;

            if (Input.GetKey(KeyCode.Mouse1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                RotInput.x = Input.GetAxisRaw("Mouse X");
                RotInput.y = Input.GetAxisRaw("Mouse Y");
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                RotInput.x = 0;
                RotInput.y = 0;
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                Launcher.Fire();
            }

            Engine.SetInputDirection(MoveInput);
            Engine.SetInputRotation(RotInput);

            CombinedRotInputs += RotInput;
            Engine.OnFrameUpdate(Time.deltaTime);
        }

        public void OnFixedUpdate()
        {
            //Engine.OnPhysicsUpdate(Time.fixedDeltaTime);
        }

        #endregion

        #region IDamageable
        [Header("Damageable")]

        public readonly SyncVar<int> _MaxHp = new(100);
        public readonly SyncVar<int> _CurrentHP = new(100);

        int IDamageable.MaxHp => _MaxHp.Value;

        int IDamageable.CurrentHp => _CurrentHP.Value;

        void IDamageable.TakeDamage(int val)
        {
            _CurrentHP.Value = Mathf.Max(0, _CurrentHP.Value - val);
            UpdateHP();
        }

        void IDamageable.HealDamage(int val)
        {
            _CurrentHP.Value = Mathf.Min(_MaxHp.Value, _CurrentHP.Value + val);
            UpdateHP();
        } 

        protected virtual void UpdateHP()
        {

        }
        #endregion
    } 
}
