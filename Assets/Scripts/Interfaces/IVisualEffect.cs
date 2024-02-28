using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Interface for any script controlling a type of VFX
    /// Generally used by object poolers
    /// </summary>
    public interface IVisualEffect
    {
        public float Lifetime {  get; }
    }
}
