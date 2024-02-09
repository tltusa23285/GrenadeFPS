using FishNet.Object.Prediction;
using UnityEngine;

/// File organizing generic/commonly used structs

namespace Game.DataStructures
{
    public enum INPUT_STATE : byte
    {
        IsUp = 0,
        IsDown = 1,
    }
    namespace Network
    {
        public struct InputData : IReplicateData
        {
            public Vector3 MoveInput;
            public Vector2 RotInput;
            public bool JumpDown;
            public bool Action1Down;

            #region IReplicateData
            private uint Tick;
            public uint GetTick() => Tick;
            public void SetTick(uint value) => Tick = value;
            public void Dispose() { }
            #endregion
        }
        public struct StateData : IReconcileData
        {
            public Vector3 Position;
            public Vector2 Rotation;
            public Vector3 Velocity;


            #region IReconcileData
            private uint Tick;
            public uint GetTick() => Tick;
            public void SetTick(uint value) => Tick = value;
            public void Dispose() { }
            #endregion
        }
    }
}
