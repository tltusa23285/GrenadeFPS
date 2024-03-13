using UnityEngine;
using UnityEngine.VFX;

namespace Game.VfxScripts
{
    [RequireComponent(typeof(VisualEffect))]
    public class VisualEffectController : VfxObject
    {
        [SerializeField] private string DurationParameter = "Lifetime";

        private VisualEffect Effect;

        private void Awake()
        {
            Effect = GetComponent<VisualEffect>();
        }
        public override float Duration
        {
            get
            {
                return base.Duration;
            }
            set
            {
                base.Duration = value;
                Effect.SetFloat(DurationParameter, value);
            }
        }
        public override void Play()
        {
            this.gameObject.SetActive(true);
            Effect.Play();
        }
        public override void Stop()
        {
            this.gameObject.SetActive(false);
            Effect.Stop();
        }
        public override void Dispose()
        {
            this.Stop();
            Destroy(this.gameObject);
        }
    }
}
