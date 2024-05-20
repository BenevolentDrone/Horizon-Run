using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.MVVM;
using HereticalSolutions.MVVM.View;

using UnityEngine;

namespace HereticalSolutions.MVVM.Mono
{
    public abstract class AViewComponent
        : MonoBehaviour
    {
        [SerializeField]
        protected AViewModelComponent baseViewModel;

        public AViewModelComponent BaseViewModel
        {
            get => baseViewModel;
            set
            {
                baseViewModel = value;

                if (view != null)
                {
                    if (view.IsSetUp)
                    {
                        view.TearDown();
                    
                        view.SetUp();

                        if (started)
                            SyncLifetimeWithVM();
                    }
                }
            }
        }

        protected AView view;

        protected bool isBeingCleanedUp = false;

        protected bool isBeingTornDown = false;

        protected bool started = false;

        #region Unity callbacks

        protected virtual void Awake()
        {
            var setUppableView = view as ISetUppable;

            setUppableView?.SetUp();


            view.OnInitialized += EnableOnInitialize;

            view.OnCleanedUp += DisableOnCleanup;

            view.OnTornDown += RemoveDelegatesOnTearDown;


            isBeingCleanedUp = false;

            isBeingTornDown = false;
        }

        protected virtual void Start()
        {
            started = true;

            SyncLifetimeWithVM();
        }

        protected void OnDisable()
        {
            if (isBeingCleanedUp)
                return;

            var cleanUppableView = view as ICleanUppable;

            cleanUppableView?.Cleanup();
        }

        protected virtual void OnDestroy()
        {
            if (isBeingTornDown)
                return;

            var tearDownableView = view as ITearDownable;

            tearDownableView?.TearDown();
        }

        #endregion

        protected void SyncLifetimeWithVM()
        {
            if (baseViewModel != null
                && baseViewModel.ViewModel != null)
            {
                LifetimeSynchronizer.SyncLifetimes(
                    view as ILifetimeable,
                    baseViewModel.ViewModel as ILifetimeable);
            }
        }

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

            view.OnInitialized -= EnableOnInitialize;

            view.OnCleanedUp -= DisableOnCleanup;

            view.OnTornDown -= RemoveDelegatesOnTearDown;

            isBeingTornDown = false;
        }
    }
}