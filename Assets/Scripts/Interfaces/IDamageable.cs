using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Interfaces
{
    public interface IDamageable
    {
        public int MaxHp {  get; }
        public int CurrentHp { get; }
        public void TakeDamage(int val);
        public void HealDamage(int val);
    }
}