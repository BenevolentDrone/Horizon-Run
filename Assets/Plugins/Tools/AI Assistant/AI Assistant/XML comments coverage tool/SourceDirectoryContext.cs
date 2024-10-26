namespace AIAssistant
{
    public class SourceDirectoryContext
    {
        private object lockObject = new object();
        
        private EFolderProcessingStatus status;

        public EFolderProcessingStatus Status
        {
            get
            {
                EFolderProcessingStatus result;
                
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

        public string Path { get; private set; }
        
        public SourceFileContext[] Files { get; private set; }

        public SourceDirectoryContext(string path)
        {
            Path = path;
        
            status = EFolderProcessingStatus.UNCHANGED;

            Files = null;
        }

        public void SetSourceFiles(SourceFileContext[] files)
        {
            lock (lockObject)
            {
                Files = files;
            }
        }
    }
}