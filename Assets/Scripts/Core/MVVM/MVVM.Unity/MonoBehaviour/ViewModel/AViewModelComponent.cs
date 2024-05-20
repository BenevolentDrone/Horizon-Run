using HereticalSolutions.LifetimeManagement;

using UnityEngine;

namespace HereticalSolutions.MVVM.Mono
{
    public abstract class AViewModelComponent
        : MonoBehaviour
    {
        #region View Model
        
        protected IViewModel viewModel;

        public IViewModel ViewModel { get => viewModel; }

        #endregion

        protected bool isBeingCleanedUp = false;

        protected bool isBeingTornDown = false;
        
        /*
        #region Hierarchy

        [SerializeField]
        protected AViewModelComponent parentVM;
        
        /// <summary>
        /// Reference to parent VM component
        /// </summary>
        public AViewModelComponent Parent { get; protected set; }

        #endregion
        */

        #region Unity callbacks
        
        protected virtual void Awake()
        {
            //if (parentVM != null)
            //    Parent = parentVM;
            
            var setUppableViewModel = viewModel as ISetUppable;

            setUppableViewModel?.SetUp();


            var lifetimeableViewModel = viewModel as ILifetimeable;

            lifetimeableViewModel.OnInitialized += EnableOnInitialize;

            lifetimeableViewModel.OnCleanedUp += DisableOnCleanup;

            lifetimeableViewModel.OnTornDown += RemoveDelegatesOnTearDown;


            isBeingCleanedUp = false;

            isBeingTornDown = false;
        }

        //The VM will be initialized by its parent VM or a bootstrapper
        //And synched to it too
        /*
        protected virtual void Start()
        {
            //Sync with parent VM if there is one
            if (Parent != null)
            {
                LifetimeSynchronizer.SyncLifetimes(
                    viewModel as ILifetimeable,
                    Parent.ViewModel as ILifetimeable);
            }
        }
        */

        protected void OnDisable()
        {
            if (isBeingCleanedUp)
                return;

            var cleanUppableViewModel = viewModel as ICleanUppable;

            cleanUppableViewModel?.Cleanup();
        }

        protected virtual void OnDestroy()
        {
            if (isBeingTornDown)
                return;

            var tearDownableViewModel = viewModel as ITearDownable;

            tearDownableViewModel?.TearDown();
        }
        
        #endregion

        protected void EnableOnInitialize()
        {
            gameObject.SetActive(true);
        }

        protected void DisableOnCleanup()
        {
            isBeingCleanedUp = true;

            gameObject.SetActive(false);

            isBeingCleanedUp = false;
        }

        protected void RemoveDelegatesOnTearDown()
        {
            isBeingTornDown = true;

            var lifetimeableViewModel = viewModel as ILifetimeable;

            lifetimeableViewModel.OnInitialized -= EnableOnInitialize;

            lifetimeableViewModel.OnCleanedUp -= DisableOnCleanup;

            lifetimeableViewModel.OnTornDown -= RemoveDelegatesOnTearDown;

            isBeingTornDown = false;
        }
    }
}