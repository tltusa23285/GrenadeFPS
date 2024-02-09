using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class Vector3Extensions
    {
        public static Vector3 FlattenX(this Vector3 v)
        {
            v.x = 0;
            return v;
        }
        public static Vector3 FlattenY(this Vector3 v)
        {
            v.y = 0;
            return v;
        }
        public static Vector3 FlattenZ(this Vector3 v)
        {
            v.z = 0;
            return v;
        }
    }
}
