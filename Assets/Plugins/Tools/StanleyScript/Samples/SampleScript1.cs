using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using HereticalSolutions;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.StanleyScript;

using UnityEngine;

public class SampleScript1 : MonoBehaviour
{
	private static StanleyEvent externalEvent;

	void Start()
	{
		string[] testScript = new []
		{
			"//Create a variable named 'a' of type int",
			"",
			"OP_PUSH_STRING \"System.Int32\"",
			"OP_PUSH_TYPE",
			"OP_PUSH_STRING \"a\"",
			"OP_ALLOC_VARIABLE",
			"",
			"//a = 2 + 3",
			"",
			"OP_PUSH_STRING \"a\"",
			"OP_PUSH_VARIABLE",
			"OP_PUSH_INT 3",
			"OP_PUSH_INT 2",
			"OP_ADD",
			"OP_COPY",
			"",
			"//Cast a to string and print it in the report",
			"",
			"OP_PUSH_STRING \"System.String\"",
			"OP_PUSH_TYPE",
			"OP_PUSH_STRING \"a\"",
			"OP_PUSH_VARIABLE",
			"OP_CAST",
			"OP_PRINT",
			"",
			"//Wait for 10000 milliseconds internally",
			"",
			"OP_PUSH_INT 10000",
			"OP_WAIT_MS",
			"",
			"//Call the external method imported into the 'WaitExternal' delegate variable with the argument 10000 to wait for 10000 milliseconds in the external method",
			"",
			"OP_PUSH_INT 10000",
			"OP_PUSH_STRING \"WaitExternal\"",
			"OP_PUSH_VARIABLE",
			"OP_DELEGATE_CALL",
			"",
			"",
			"//Call the external method imported into the 'WaitThenRaiseEventExternal' delegate variable with the argument of event to wait for 10000 milliseconds in the external method and then raise the event",
			"",
			"OP_PUSH_STRING \"WaitThenRaiseEventExternal\"",
			"OP_PUSH_VARIABLE",
			"OP_DELEGATE_CALL",
			"",
			"OP_PUSH_STRING \"OnExternalRaised\"",
			"OP_PUSH_VARIABLE",
			"OP_WAIT_EVENT",
			"",
			"//Finish",
			"",
			"OP_RETURN"
		};

		StanleyContext context = StanleyFactory.BuildStanleyContext(
			null,
			true,
			false,
			null);

		/*
		IEnumerable<IStanleyInstructionHandler> handlers =
			typeof(IStanleyInstructionHandler)
			.Assembly
			.GetTypes()
			.Where(t =>
				t.IsSubclassOf(typeof(AStanleyInstructionHandler))
				&& !t.IsAbstract)
			.Select(type =>
				Activator.CreateInstance(
					type,
					new object[]
					{
						null,
						null
					})
				as IStanleyInstructionHandler);

		foreach (IStanleyInstructionHandler handler in handlers)
		{
			context.StackMachine.AddHandler(
				handler);
		}
		*/

		//context.SaveProgramToLibrary(
		//	"test",
		//	testScript);
		//
		//context.LoadProgramFromLibrary(
		//	"test");

		context.StackMachine.LoadInstructions(
			testScript);

		var importer = context.StackMachine.GlobalScope.Variables as IVariableImporter;

		importer.ImportDelegate(
			string.Empty,
			"WaitExternal",
			null,
			GetType().GetMethod(
				nameof(WaitExternalAsync),
				BindingFlags.Static | BindingFlags.Public),
			true,
			out _);

		importer.ImportEvent(
			string.Empty,
			"OnExternalRaised",
			null,
			false,
			string.Empty,
			out var externalEventVariable);

		externalEvent = externalEventVariable.GetValue<StanleyEvent>();

		importer.ImportDelegate(
			string.Empty,
			"WaitThenRaiseEventExternal",
			null,
			GetType().GetMethod(
				nameof(WaitThenRaiseEventAsync),
				BindingFlags.Static | BindingFlags.Public),
			false,
			out _);

		context.StartAsync(
			new AsyncExecutionContext
			{
			});

		//DONT USE UNLESS YOU WANT THE MAIN THREAD FROOOOOZEN
		//HereticalSolutions.TaskExtensions.RunSync(
		//	() => context.StartAsync(
		//		new AsyncExecutionContext
		//		{
		//		}));

		UnityEngine.Debug.Log(
			"Scenario has been started");
	}

	public static async Task WaitExternalAsync(
		int milliseconds)
	{
		UnityEngine.Debug.Log(
			$"[WaitExternalAsync] Waiting for {milliseconds} milliseconds");

		await Task.Delay(
			milliseconds);

		UnityEngine.Debug.Log(
			$"[WaitExternalAsync] Done waiting for {milliseconds} milliseconds");
	}

	public static async Task WaitThenRaiseEventAsync()
	{
		UnityEngine.Debug.Log(
			$"[WaitThenRaiseEventAsync] Waiting for 10000 milliseconds");

		await Task.Delay(
			10000);

		UnityEngine.Debug.Log(
			$"[WaitThenRaiseEventAsync] Done waiting for 10000 milliseconds, raising event");

		externalEvent.Raise();
	}
}