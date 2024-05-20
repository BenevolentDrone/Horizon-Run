using System.Collections.Generic;

namespace HereticalSolutions.StanleyScript
{
	public static class StanleyFactory
	{
		public static StanleyInterpreter BuildInterpreter()
		{
			StanleyASTWalker walker = new StanleyASTWalker(
				new StanleyEnvironment(
					new Dictionary<string, IStanleyVariable>(),
					new Dictionary<string, List<IStanleyOperation>>(),
					new Dictionary<string, IStanleyVariable>(),
					new Stack<IStanleyVariable>(),
					new List<string>()),
				new List<string>());

			//IRuntimeEnvironment environment = BuildEnvironment();

			return new StanleyInterpreter(
				walker); //,
				//environment);
		}

		public static StanleyEnvironment BuildEnvironment()
		{
			var result = new StanleyEnvironment(
				new Dictionary<string, IStanleyVariable>(),
				new Dictionary<string, List<IStanleyOperation>>(),
				new Dictionary<string, IStanleyVariable>(),
				new Stack<IStanleyVariable>(),
				new List<string>());

			IRuntimeEnvironment environment = result;

			#region No args

			environment.LoadOperation(
				new PushString());

			environment.LoadOperation(
				new PushInt());

			environment.LoadOperation(
				new PushFloat());

			#endregion

			#region One arg

			environment.LoadOperation(
				new SetLineIndex());

			environment.LoadOperation(
				new PushImportVariable());

			environment.LoadOperation(
				new PushRuntimeVariable());

			environment.LoadOperation(
				new PushStackVariable());

			environment.LoadOperation(
				new FlushStackVariable());

			environment.LoadOperation(
				new TryCastToRuntimeVariable());

			environment.LoadOperation(
				new ReadStory());

			#endregion

			#region Multiple args

			environment.LoadOperation(
				new AllocateRuntimeVariable());

			environment.LoadOperation(
				new ConcatenateToScalarObject());

			environment.LoadOperation(
				new ConcatenateVariables());

			environment.LoadOperation(
				new InvokeCommand());

			#endregion

			return result;
		}
	}
}