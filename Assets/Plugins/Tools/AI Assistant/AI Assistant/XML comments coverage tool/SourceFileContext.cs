namespace AIAssistant
{
    public class SourceFileContext
    {
        private object lockObject = new object();

        private EFileProcessingStatus status;

        public EFileProcessingStatus Status
        {
            get
            {
                EFileProcessingStatus result;
                
                lock (lockObject)
                {
                    result = status;
                }

                return result;
            }
            set
            {
                lock (lockObject)
                {
                    status = value;
                }
            }
        }

        public string FilePath { get; private set; }

        public SourceFileContext(string filePath)
        {
            status = EFileProcessingStatus.UNCHANGED;

            FilePath = filePath;
        }
    }
}