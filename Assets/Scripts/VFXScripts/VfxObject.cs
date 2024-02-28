using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.VfxScripts
{
    public abstract class VfxObject : MonoBehaviour
    {
        private float _lifeTime;
        /// <summary>
        /// How long this effect should linger, effect on appearance varies with implementation
        /// </summary>
        public virtual float Duration
        {
            get
            {
                return _lifeTime;
            }
            set
            {
                _lifeTime = value;
            }
        }

        /// <summary>
        /// Start playing the effect
        /// </summary>
        public abstract void Play();

        /// <summary>
        /// Stop playing the effect
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Flags the effect to no longer be used, such as destroying the gameobject
        /// </summary>
        public abstract void Dispose();
    }
}
