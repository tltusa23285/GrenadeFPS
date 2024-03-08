using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapons.Grenades
{
    public interface IGCompOnUpdate
    {
        public enum UpdateCategory { Frame, Physics}
        public UpdateCategory Category { get; }
        public void OnUpdate(in float deltaTime);
    }
}
