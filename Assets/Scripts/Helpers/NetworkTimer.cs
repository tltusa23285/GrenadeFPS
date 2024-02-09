using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class NetworkTimer
    {
        float Timer;
        public float MinTimeBetweenTicks { get; }
        public int CurrerntTick {  get; private set; }

        public NetworkTimer(float serverTickRate)
        {
            MinTimeBetweenTicks = 1f / serverTickRate;
        }

        public void Update(in float deltaTime)
        {
            Timer += deltaTime;
        }

        public bool ShouldTick()
        {
            if (Timer >= MinTimeBetweenTicks)
            {
                Timer -= MinTimeBetweenTicks;
                CurrerntTick++;
                return true;
            }
            return false;
        }
    }
}
