using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AddressableSelectorAttribute : PropertyAttribute
    {
        public readonly HashSet<string> Groups;
        public AddressableSelectorAttribute(params string[] groupFilter) 
        { 
            Groups = new HashSet<string>(groupFilter);
        }
    }
}
