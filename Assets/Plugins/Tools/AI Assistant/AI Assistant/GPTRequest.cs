using System.Net;

namespace AIAssistant
{
    public class GPTRequest
    {
        public HttpWebRequest HTTPRequest;
        
        public byte[] Data;

        public bool Retry;

        public string Response;

        public SourceFileContext SourceFileContext;
    }
}