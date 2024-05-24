using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
    [ViewComponent]
    public class TransformQuaternionViewComponent : AMonoViewComponent
    {
        public Transform QuaternionPivotTransform;

        public Quaternion Quaternion;

        public bool Dirty;
    }
}