namespace HereticalSolutions.Messaging
{
    public interface IMessage
    {
        void Write(
            object[] args);
    }
}