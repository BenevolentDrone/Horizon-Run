using UnityEngine;

namespace HereticalSolutions.Entities
{
    public class ViewEntityReferenceBehaviour : MonoBehaviour
    {
        [SerializeField] 
        private GameObjectViewEntityAdapter viewAdapter;

        public GameObjectViewEntityAdapter ViewAdapter { get => viewAdapter; }
    }
}