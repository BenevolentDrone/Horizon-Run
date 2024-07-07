using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity
{
	[CreateAssetMenu(fileName = "Entity authoring settings", menuName = "Settings/Entities/Entity authoring settings", order = 0)]
	public class EntityAuthoringSettingsScriptable : ScriptableObject
	{
		public EntityAuthoringSettings EntityAuthoringSettings;
	}
}