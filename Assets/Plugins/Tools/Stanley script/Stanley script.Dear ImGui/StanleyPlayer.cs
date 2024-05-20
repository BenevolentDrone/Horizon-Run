using System;
using System.Numerics;
using ImGuiNET;

namespace HereticalSolutions.StanleyScript
{
	public class StanleyPlayer
	{
		private readonly IStanleyInterpreter interpreter;

		private readonly IExecutable executable;

		private readonly IReportable reportable;

		private readonly ScenarioNameScriptPair[] scenarioPlaylist;


		private string currentScenarioName;

		private string currentScenario;

		public StanleyPlayer(
			IStanleyInterpreter interpreter,
			IExecutable executable,
			ScenarioNameScriptPair[] scenarioPlaylist)
		{
			this.interpreter = interpreter;

			this.scenarioPlaylist = scenarioPlaylist;

			this.executable = executable;

			this.reportable = executable as IReportable;


			currentScenarioName = string.Empty;

			currentScenario = string.Empty;
		}

		public void ShowWindow()
		{
			// Main body of the Demo window starts here.
			if (!ImGui.Begin("Stanley script playback"))
			{
				// Early out if the window is collapsed, as an optimization.
				ImGui.End();

				return;
			}

			// Most "big" widgets share a common width settings by default. See 'Demo->Layout->Widgets Width' for details.
			// e.g. Use 2/3 of the space for widgets and 1/3 for labels (right align)
			//ImGui::PushItemWidth(-ImGui::GetWindowWidth() * 0.35f);
			// e.g. Leave a fixed amount of width for labels (by passing a negative value), the rest goes to widgets.
			ImGui.PushItemWidth(ImGui.GetFontSize() * -12);

			DrawPlaybackControls();

			ImGui.SeparatorText("Current scenario");

			if (!string.IsNullOrEmpty(currentScenarioName))
			{
				ImGui.BulletText($"Name: {currentScenarioName}");
			}

			DrawScenarioListing();

			DrawOpcodesListing();

			DrawReportListing();

			ImGui.SeparatorText("Playlist");

			DrawScenarioPlaylist();

			// End of ShowDemoWindow()
			ImGui.PopItemWidth();
			ImGui.End();
		}

		private void DrawPlaybackControls()
		{
			ImGui.SeparatorText("Playback controls");

			ImGui.Text($"Status: {executable.Status}");

			ImGui.SameLine();

			string pcText = $"Program counter: {executable.ProgramCounter}";

			ImGui.SetCursorPosX(
				ImGui.GetCursorPosX()
				+ ImGui.GetColumnWidth()
				- ImGui.CalcTextSize(pcText).X);
				//- ImGui.GetStyle().ItemSpacing.X);

			ImGui.Text($"Program counter: {executable.ProgramCounter}");

			DrawStartButton();

			ImGui.SameLine();

			DrawStepButton();

			ImGui.SameLine();

			DrawPauseButton();

			ImGui.SameLine();

			DrawResumeButton();

			ImGui.SameLine();

			DrawStopButton();
		}

		private void DrawStartButton()
		{
			if (ImGui.Button("Start"))
			{
				executable.Start();
			}
		}

		private void DrawStepButton()
		{
			if (ImGui.Button("Step"))
			{
				executable.Step();
			}
		}

		private void DrawPauseButton()
		{
			if (ImGui.Button("Pause"))
			{
				executable.Pause();
			}
		}

		private void DrawResumeButton()
		{
			if (ImGui.Button("Resume"))
			{
				executable.Resume();
			}
		}

		private void DrawStopButton()
		{
			if (ImGui.Button("Stop"))
			{
				executable.Stop();
			}
		}

		private void DrawScenarioListing()
		{
			if (!ImGui.CollapsingHeader("Scenario listing"))
				return;

			if (!string.IsNullOrEmpty(currentScenarioName))
			{
				ImGui.BeginChild(
					"Scenario listing",
					new Vector2(
						ImGui.GetContentRegionAvail().X,
						300));

				string[] lines = currentScenario.Split('\n'); //executable.Instructions;

				int currentLine = executable.CurrentLine - 1; //-1 because ANTLR4's AST starts with line 1, not line 0

				if (lines != null)
				{
					for (int i = 0; i < lines.Length; i++)
					{
						string lineText = $"{i}: {lines[i]}";

						if (currentLine == i)
							lineText = $"> {lineText}";

						ImGui.Text(lineText);
					}
				}

				ImGui.EndChild();
			}
		}

		private void DrawReportListing()
		{
			if (!ImGui.CollapsingHeader("Report listing"))
				return;

			var report = reportable.GetReport();

			if (report != null)
			{
				ImGui.BeginChild(
					"Report listing",
					new Vector2(
						ImGui.GetContentRegionAvail().X,
						300));

				for (int i = 0; i < report.Length; i++)
				{
					ImGui.Text($"{i}: {report[i]}");
				}

				ImGui.EndChild();
			}
		}

		private void DrawOpcodesListing()
		{
			if (!ImGui.CollapsingHeader("Opcodes listing"))
				return;

			if (!string.IsNullOrEmpty(currentScenarioName))
			{
				ImGui.BeginChild(
					"Opcodes listing",
					new Vector2(
						ImGui.GetContentRegionAvail().X,
						300));

				string[] instructions = executable.Instructions;

				if (instructions != null)
				{
					for (int i = 0; i < instructions.Length; i++)
					{
						ImGui.Text($"{i}: {instructions[i]}");
					}
				}

				ImGui.EndChild();
			}
		}

		private void DrawScenarioPlaylist()
		{
			if (!ImGui.CollapsingHeader("Scenario playlist"))
				return;

			for (int i = 0; i < scenarioPlaylist.Length; i++)
			{
				if (ImGui.Button($"Load {scenarioPlaylist[i].Name}"))
				{
					currentScenarioName = scenarioPlaylist[i].Name;

					currentScenario = scenarioPlaylist[i].Script;

					var instructions = interpreter.InterpretToOpcode(scenarioPlaylist[i].Script);
					
					executable.LoadProgram(instructions);
				}
			}
		}
	}
}