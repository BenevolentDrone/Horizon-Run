namespace AIAssistant
{
    public enum EFolderProcessingStatus
    {
        UNCHANGED,
        
        RETRIEVING_SOURCE_FILES,
        
        PROMPTING,
        
        AWAITING_WRITE,
        
        DONE,
        
        ABORTED
    }
}