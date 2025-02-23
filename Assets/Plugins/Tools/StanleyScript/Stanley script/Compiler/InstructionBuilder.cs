using System;
using System.Collections.Generic;
using System.Text;

using Antlr4.Runtime.Misc;

namespace HereticalSolutions.StanleyScript
{
    public class InstructionBuilder
        : IInstructionBuilder
    {
        private StringBuilder stringBuilder;
        
        private int programCounter;
        
        public InstructionBuilder(
            StringBuilder stringBuilder)
        {
            this.stringBuilder = stringBuilder;
            
            programCounter = 0;
        }

        public StringBuilder StringBuilder => stringBuilder;

        public int InstructionsCount
        {
            get => programCounter;
            set => programCounter = value;
        }
        
        public string[] Instructions
        {
            get
            {
                return stringBuilder
                    .ToString()
                    .Split(
                        new string[]
                        {
                            Environment.NewLine
                        },
                        StringSplitOptions.None);
            }
        }
    }
}