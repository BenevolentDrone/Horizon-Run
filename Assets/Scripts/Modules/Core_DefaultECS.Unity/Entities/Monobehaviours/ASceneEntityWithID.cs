using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public abstract class ASceneEntityWithID<TEntityID> : ASceneEntity
	{
		[SerializeField] // FOR DEBUG PURPOSES ONLY
		protected string entityID;

		public abstract TEntityID EntityID { get; }

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (string.IsNullOrEmpty(entityID))
            {
				entityID = AllocateID().ToString();

                UnityEditor.Undo.RecordObject(this, "Assigned entity ID");
            }

            EditorUtility.SetDirty(this);
        }

		protected abstract TEntityID AllocateID();
#endif
	}
}