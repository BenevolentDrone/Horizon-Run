using System;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    [Serializable]
    public class CursorData
    {
        public string Name;

        public Texture2D Texture;
        
        public Vector2 Hotspot;
    }
}