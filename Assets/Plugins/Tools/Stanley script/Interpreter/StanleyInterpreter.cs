using HereticalSolutions.StanleyScript.Grammars;

using Antlr4.Runtime;

namespace HereticalSolutions.StanleyScript
{
    public class StanleyInterpreter
        : IStanleyInterpreter
    {
        private readonly StanleyASTWalker walker;

        public StanleyInterpreter(
            StanleyASTWalker walker)
        {
            this.walker = walker;
        }

        public string[] InterpretToOpcode(string script)
        {
            AntlrInputStream inputStream = new AntlrInputStream(script);

            StanleyLexer StanleyLexer = new StanleyLexer(inputStream);

            CommonTokenStream commonTokenStream = new CommonTokenStream(StanleyLexer);

            StanleyParser StanleyParser = new StanleyParser(commonTokenStream);

            StanleyParser.ScriptContext context = StanleyParser.script();

            walker.Initialize();

            walker.Visit(context);

            var instructions = walker.GetInstructions();

            walker.Initialize();

            return instructions;
        }
    }
}