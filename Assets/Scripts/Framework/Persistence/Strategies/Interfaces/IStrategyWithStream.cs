using System;
using System.Threading;
using System.Threading.Tasks;

using System.IO;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public interface IStrategyWithStream
		: IAsyncSerializationStrategy,
		  IStrategyWithState,
		  IBlockSerializationStrategy,
		  IAsyncBlockSerializationStrategy
	{
		EStreamMode CurrentMode { get; }

		Stream Stream { get; }

		bool StreamOpen { get; }


		#region Flush

		bool FlushAutomatically { get; }

		void Flush();

		Task FlushAsync(
			
			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		#endregion


		#region Seek

		long Position { get; }

		bool CanSeek { get; }

		bool Seek(
			long offset,
			out long position);

		bool SeekFromStart(
			long offset,
			out long position);

		bool SeekFromFinish(
			long offset,
			out long position);

		#endregion
	}
}