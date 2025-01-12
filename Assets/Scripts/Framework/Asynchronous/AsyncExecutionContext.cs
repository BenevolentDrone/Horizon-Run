using System;
using System.Threading;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Asynchronous
{
	public class AsyncExecutionContext
	{
		public CancellationToken CancellationToken; // { get; private set; }

		public IProgress<float> Progress; // { get; private set; }

		public ILogger ProgressLogger; // { get; private set; }
	}
}