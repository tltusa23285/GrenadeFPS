using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Interfaces
{
    /// <summary>
    /// Interface for objects which can have a force applied to them
    /// For most cases this can be considered the same as applying forces to rigidbodies
    /// Interface is to allow for implementers to modify incoming forces, or for cases where we want to apply force to an object not using a rigidbody
    /// </summary>
    public interface IForceable
    {
        public void AddForce(Vector3 force, ForceMode mode);
        public void AddExplosionForce(float force, Vector3 origin, float radius, ForceMode mode);


        // TODO : we shouldnt need this kind of splitting, but keeping as reference 
        //public void AddForce(Vector3 force) => AddForce(force.x,force.y,force.z);
        //public void AddForce(float x, float y, float z);
    }
}
