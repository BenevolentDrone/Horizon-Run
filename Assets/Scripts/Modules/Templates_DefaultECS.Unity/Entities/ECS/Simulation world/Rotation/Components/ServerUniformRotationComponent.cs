namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public struct ServerUniformRotationComponent
    {
        public float ServerRotation;
        
        public float Error;

        public ushort ServerTick;

        public bool Dirty;
    }
}