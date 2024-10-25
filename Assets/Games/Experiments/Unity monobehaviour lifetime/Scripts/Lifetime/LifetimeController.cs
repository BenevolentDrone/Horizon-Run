using System;

namespace HeresySolutions.Lifetime
{
    public class LifetimeController
    {
        private Action setUpDelegate;
        
        private Action establishConnectionsDelegate;
        
        private Action initializeDelegate;
        
        private Action cleanUpDelegate;
        
        private Action tearDownDelegate;
        
        
        public bool IsSetUp { get; private set; }
        
        public bool ConnectionsEstablished { get; private set; }
        
        public bool IsInitialized { get; private set; }
        
        
        public Action OnSetUp { get; set; }
        
        public Action OnConnectionsEstablished { get; set; }
        
        public Action OnInitialized { get; set; }
        
        public Action OnCleanedUp { get; set; }
        
        public Action OnTornDown { get; set; }
        
        
        public LifetimeController(
            Action setUpDelegate = null,
            Action establishConnectionsDelegate = null,
            Action initializeDelegate = null,
            Action cleanUpDelegate = null,
            Action tearDownDelegate = null)
        {
            this.setUpDelegate = setUpDelegate;
            
            this.establishConnectionsDelegate = establishConnectionsDelegate;
            
            this.initializeDelegate = initializeDelegate;
            
            this.cleanUpDelegate = cleanUpDelegate;
            
            this.tearDownDelegate = tearDownDelegate;
        }
        
        
        public void SetUp()
        {
            if (IsSetUp)
                return;
            
            setUpDelegate?.Invoke();
            
            IsSetUp = true;
            
            OnSetUp?.Invoke();
        }
        
        public void EstablishConnections()
        {
            if (ConnectionsEstablished)
                return;
            
            if (setUpDelegate != null
                && !IsSetUp)
                throw new Exception("[LifetimeController] Attempt to establish connections on lifetimeable that is not set up yet");
            
            establishConnectionsDelegate?.Invoke();
            
            ConnectionsEstablished = true;
            
            OnConnectionsEstablished?.Invoke();
        }
        
        public void Initialize()
        {
            if (IsInitialized)
                return;
            
            if (setUpDelegate != null
                && !IsSetUp)
                throw new Exception("[LifetimeController] Attempt to initialize a lifetimeable that is not set up yet");
            
            if (establishConnectionsDelegate != null
                && !ConnectionsEstablished)
                throw new Exception("[LifetimeController] Attempt to initialize a lifetimeable that haven't established connections yet");
            
            initializeDelegate?.Invoke();
            
            IsInitialized = true;
            
            OnInitialized?.Invoke();
        }
        
        public void CleanUp()
        {
            if (!IsInitialized)
                return;
            
            cleanUpDelegate?.Invoke();
            
            IsInitialized = false;
            
            OnCleanedUp?.Invoke();
        }
        
        public void TearDown()
        {
            if (!IsSetUp)
                return;
            
            if (IsInitialized)
                CleanUp();
            
            tearDownDelegate?.Invoke();
            
            IsSetUp = false;
            
            ConnectionsEstablished = false;
            
            OnTornDown?.Invoke();
        }
    }
}