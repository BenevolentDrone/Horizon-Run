using System;

namespace HereticalSolutions.StanleyScript
{
	public class StanleyEvent
		: IClonable
	{
		private readonly Func<bool> poller;

		private bool raised = false;

		public bool Raised => raised;

		private bool jumpToLabel = true;

		public bool JumpToLabel => jumpToLabel;

		private string label;

		public string Label
		{
			get => label;
			set => label = value;
		}

		public StanleyEvent(
			Func<bool> poller,
			bool jumpToLabel,
			string label)
		{
			this.poller = poller;

			this.label = label;

			this.jumpToLabel = jumpToLabel;
		}

		public void Poll()
		{
			if (raised)
				return;

			if (poller != null)
				raised = poller.Invoke();
		}

		public void Raise()
		{
			raised = true;
		}

		public void Reset()
		{
			raised = false;
		}

		#region IClonable

		public object Clone()
		{
			return StanleyFactory.BuildStanleyEvent(
				poller,
				jumpToLabel,
				label);
		}

		#endregion
	}
}