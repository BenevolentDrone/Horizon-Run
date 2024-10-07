using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    public class ViewEntityReferenceBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private GameObjectViewEntityAdapter viewAdapter;

        public GameObjectViewEntityAdapter ViewAdapter { get => viewAdapter; }
    }
}