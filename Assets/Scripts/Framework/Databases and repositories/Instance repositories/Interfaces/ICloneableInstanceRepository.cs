namespace HereticalSolutions.Repositories
{
    public interface ICloneableInstanceRepository
    {
        IInstanceRepository Clone();
    }
}