namespace AIAssistant
{
    public enum EFileProcessingStatus
    {
        UNCHANGED,
        
        AWAITING_TIMEOUT,
        
        PROMPTING,
        
        AWAITING_WRITE,
        
        DONE,
        
        TOO_LONG,
        
        ABORTED,
        
        ERROR,
        
        EXCEPTION,
        
        TOKENS_EXCEEDED
    }
}