namespace HereticalSolutions.StateMachines
{
    public class SceneState : IState
    {
        public string SceneID { get; protected set; }

        public bool SetAsActiveOnLoad { get; protected set; }

        public SceneState(
            string sceneID,
            bool setAsActiveOnLoad = true)
        {
            SceneID = sceneID;

            SetAsActiveOnLoad = setAsActiveOnLoad;
        }

        public virtual void OnStateEntered()
        {
        }

        public virtual void OnStateExited()
        {            
        }
    }
}