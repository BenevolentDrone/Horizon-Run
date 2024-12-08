using UnityEngine;

namespace HereticalSolutions.Lifetime
{
    public class SmartObject : Component
    {
        public SmartObject()
        {
            Debug.Log($"[SmartObject] Constructor call on gameobject {name}");
        }
    }
}