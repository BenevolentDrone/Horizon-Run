using DefaultEcs;

using UnityEngine;

//#if UNITY_EDITOR
//
//using System;
//
//#endif

namespace HereticalSolutions.Entities
{
	public class GameObjectViewEntityAdapter
		: MonoBehaviour
	{
		[SerializeField]
		private AMonoViewComponent[] viewComponents;

		private bool initialized;

		public bool Initialized
		{
			get => initialized;
		}

		public AMonoViewComponent[] GetViewComponents { get => viewComponents; }

		private Entity viewEntity;

		public Entity ViewEntity
		{
			get
			{
				if (!initialized)
					return default(Entity);

				return viewEntity;
			}
		}

//#if UNITY_EDITOR
//        
//        [SerializeField]
//        private string GUID;
//
//#endif

		public void Initialize(Entity viewEntity)
		{
			this.viewEntity = viewEntity;

			foreach (var viewComponent in viewComponents)
			{
				viewComponent.Install(viewEntity);
			}

//#if UNITY_EDITOR
//            GUID = viewEntity.Get<GUIDComponent>().GUID.ToString();
//#endif

			initialized = true;
		}

		public void Deinitialize()
		{
			this.viewEntity = default(Entity);

//#if UNITY_EDITOR
//            GUID = string.Empty;            
//#endif

			initialized = false;
		}
	}
}