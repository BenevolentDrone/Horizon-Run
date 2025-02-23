using System;
using System.Collections.Generic;
using System.Text;

using Antlr4.Runtime.Misc;

namespace HereticalSolutions.StanleyScript
{
    public interface IInstructionBuilder
    {
        StringBuilder StringBuilder { get; }

        int InstructionsCount { get; set; }
        
        string[] Instructions { get; }
    }
}