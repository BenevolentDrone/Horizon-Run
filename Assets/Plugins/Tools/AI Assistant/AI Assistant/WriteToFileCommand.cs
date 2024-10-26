using System;

namespace AIAssistant
{
    [Serializable]
    public struct WriteToFileCommand
    {
        public SourceFileContext Context;

        public string Text;
    }
}