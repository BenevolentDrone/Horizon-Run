using System;
using System.Collections.Generic;

namespace HereticalSolutions.StanleyScript
{
    /// <summary>
    /// Post-processes the generated instructions after the AST walk is complete.
    /// Handles tasks like resolving label addresses and any future post-processing steps.
    /// </summary>
    public class StanleyPostProcessor
    {
        private readonly string[] instructions;

        public StanleyPostProcessor(
            string[] instructions)
        {
            this.instructions = instructions;
        }

        /// <summary>
        /// Runs all post-processing steps on the generated instructions
        /// </summary>
        public string[] Process()
        {
            ReplaceLabelPlaceholdersWithAddresses();
            
            // Future post-processing steps will be added here
            return instructions;
        }

        /// <summary>
        /// Replaces label address placeholders with their actual instruction addresses.
        /// </summary>
        private void ReplaceLabelPlaceholdersWithAddresses()
        {
            var labelAddresses = FillLabelAddressTable();
            ReplaceLabelPlaceholders(labelAddresses);
        }

        /// <summary>
        /// First pass: builds a table mapping label names to their instruction addresses
        /// </summary>
        private Dictionary<string, int> FillLabelAddressTable()
        {
            var labelAddresses = new Dictionary<string, int>();

            for (int i = 0; i < instructions.Length; i++)
            {
                var trimmed = instructions[i].Trim();
                
                if (trimmed.StartsWith(StanleyOpcodes.LABEL))
                {
                    string label = trimmed.Substring(StanleyOpcodes.LABEL.Length).Trim();
                    
                    labelAddresses[label] = i;
                }
            }

            return labelAddresses;
        }

        /// <summary>
        /// Second pass: replaces label address placeholders with actual addresses
        /// </summary>
        private void ReplaceLabelPlaceholders(Dictionary<string, int> labelAddresses)
        {
            for (int i = 0; i < instructions.Length; i++)
            {
                var line = instructions[i].Trim();
                
                if (line.StartsWith($"{StanleyOpcodes.OP_PUSH_INT} <ADDR_"))
                {
                    string label = line.Substring(
                        $"{StanleyOpcodes.OP_PUSH_INT} <ADDR_".Length,
                        line.Length - $"{StanleyOpcodes.OP_PUSH_INT} <ADDR_".Length - 1);
                    
                    if (labelAddresses.TryGetValue(label, out int address))
                    {
                        instructions[i] = $"{StanleyOpcodes.OP_PUSH_INT} {address}";
                    }
                }
            }
        }
    }
}
