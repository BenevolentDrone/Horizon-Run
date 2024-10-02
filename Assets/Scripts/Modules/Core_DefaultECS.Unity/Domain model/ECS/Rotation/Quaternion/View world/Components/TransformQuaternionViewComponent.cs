using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    [ViewComponent]
    public class TransformQuaternionViewComponent : AMonoViewComponent
    {
        public Transform QuaternionPivotTransform;

        public Quaternion Quaternion;

        public bool Dirty;
    }
}