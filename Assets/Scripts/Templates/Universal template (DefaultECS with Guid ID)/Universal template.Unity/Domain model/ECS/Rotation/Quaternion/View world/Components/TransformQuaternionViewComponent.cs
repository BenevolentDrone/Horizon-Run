using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity
{
    [ViewComponent]
    public class TransformQuaternionViewComponent : AMonoViewComponent
    {
        public Transform QuaternionPivotTransform;

        public Quaternion Quaternion;

        public bool Dirty;
    }
}