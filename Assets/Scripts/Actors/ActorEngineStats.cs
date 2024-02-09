using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Actors.Data
{
    /// <summary>
    /// Base data class for actor engine
    /// </summary>
    [System.Serializable]
    public class ActorEngineStats
    {
        [Header("Movement")]
        public float MoveSpeed = 10;
        public float Acceleration = 25;

        [Header("Look")]
        public float LookSpeed = 1;
        public float VertLookClamp = 85;

        [Header("Jumping")]
        public float JumpStrength = 5;
        public float JumpGravityMulti = 2.5f;
        public float JumpIgnoreGroundTime = 0.1f;

        [Header("Gravity")]
        public float GravityAcceleration = 9.8f;
        public float FallGravityMulti = 5f;

        [Header("Grounded Checks")]
        public float HeadHeight = 2;
        public LayerMask GroundedMask;
        public float GroundSphereCastRadius = 0.25f;
    }
}