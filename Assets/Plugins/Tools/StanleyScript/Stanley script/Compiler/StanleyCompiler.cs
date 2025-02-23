using System;

//using HereticalSolutions.StanleyScript.Grammars;

using Antlr4.Runtime;

namespace HereticalSolutions.StanleyScript
{
    public class StanleyCompiler
        : IStanleyCompiler
    {
        private readonly StanleyASTWalker walker;

        private string[] programListing;

        public StanleyCompiler(
            StanleyASTWalker walker)
        {
            this.walker = walker;

            programListing = Array.Empty<string>();
        }

        #region IStanleyCompiler

        public string[] ProgramListing => programListing;

        public void LoadProgram(
            string[] program)
        {
            programListing = program;
        }

        public bool Compile(
            out string[] instructions)
        {
            AntlrInputStream inputStream = new AntlrInputStream(
                String.Join('\n', programListing));

            StanleyLexer StanleyLexer = new StanleyLexer(inputStream);

            CommonTokenStream commonTokenStream = new CommonTokenStream(StanleyLexer);

            StanleyParser StanleyParser = new StanleyParser(commonTokenStream);

            StanleyParser.ProgramContext programContext = StanleyParser.program();

            //walker.Initialize();

            walker.ProgramListing = programListing;
            
            walker.Visit(programContext);

            // Post-process the instructions
            var postProcessor = new StanleyPostProcessor(
                walker.Instructions);
            
            instructions = postProcessor.Process();

            return true;
        }

        public bool CompileImmediately(
            string programLine,
            out string[] instructions)
        {
            AntlrInputStream inputStream = new AntlrInputStream(
                programLine);

            StanleyLexer StanleyLexer = new StanleyLexer(inputStream);

            CommonTokenStream commonTokenStream = new CommonTokenStream(StanleyLexer);

            StanleyParser StanleyParser = new StanleyParser(commonTokenStream);

            StanleyParser.ProgramContext programContext = StanleyParser.program();

            //walker.Initialize();

            walker.ProgramListing = new[] { programLine };
            
            walker.Visit(programContext);

            instructions = walker.Instructions;

            //walker.Initialize();

            return true;
        }

        #endregion
    }
}