using UnityEngine;
using ImGuiNET;
using UImGui;
using System.Collections.Generic;
using System;
using System.Threading;

namespace HereticalSolutions.StanleyScript
{
	[RequireComponent(typeof(UImGuiBehaviour))]
	public class StanleyPlayerMono : MonoBehaviour
	{
		[SerializeField]
		private MonoBehaviour stanleyScriptBehaviour;

		[SerializeField]
		private TextAsset[] scenarioAssets;

		private StanleyPlayer stanleyPlayer;

		private ScenarioNameScriptPair[] scenarios;

		void Start()
		{
			IStanleyScriptMono stanleyMonoBehaviour = stanleyScriptBehaviour as IStanleyScriptMono;

			ParseScenarioAssets(stanleyMonoBehaviour.Interpreter);

			stanleyPlayer = new StanleyPlayer(
				stanleyMonoBehaviour.Interpreter,
				stanleyMonoBehaviour.Environment as IExecutable,
				scenarios);
		}

		private void ParseScenarioAssets(IStanleyInterpreter interpreter)
		{
			var scenariosList = new List<ScenarioNameScriptPair>();

			for (int i = 0; i < scenarioAssets.Length; i++)
			{
				var instructions = interpreter.InterpretToOpcode(scenarioAssets[i].text);

				int storyInstructionIndex = Array.IndexOf(instructions, "OP_STORY");

				if (storyInstructionIndex == -1)
				{
					Debug.LogError($"SCENARIO ASSET {scenarioAssets[i].name}: NOT A STANLEY SCRIPT COMPLIANT STORY");
				}

				int storyNameIndex = storyInstructionIndex;

				while (!instructions[storyNameIndex].StartsWith("OP_PUSH_STR"))
				{
					if (storyNameIndex == -1)
					{
						Debug.LogError($"SCENARIO ASSET {scenarioAssets[i].name}: NOT A STANLEY SCRIPT COMPLIANT STORY");
					}

					storyNameIndex--;
				}

				var scenarioName = instructions[storyNameIndex].Split(' ')[1];

				scenariosList.Add(
					new ScenarioNameScriptPair
					{
						Name = scenarioName,

						Script = scenarioAssets[i].text
					});
			}

			scenarios = scenariosList.ToArray();
		}

		private void OnEnable()
		{
			UImGuiUtility.Layout += OnLayout;
		}

		private void OnDisable()
		{
			UImGuiUtility.Layout -= OnLayout;
		}

		private void OnLayout(UImGuiBehaviour uImGui)
		{
			stanleyPlayer.ShowWindow();
		}
	}
}

