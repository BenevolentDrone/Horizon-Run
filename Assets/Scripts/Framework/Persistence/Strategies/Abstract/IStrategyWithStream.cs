using System.IO;
using System.Threading.Tasks;

namespace HereticalSolutions.Persistence
{
	public interface IStrategyWithStream
		: IAsyncSerializationStrategy,
		  IStrategyWithState,
		  IBlockSerializationStrategy,
		  IAsyncBlockSerializationStrategy
	{
		ESreamMode CurrentMode { get; }

		Stream Stream { get; }

		bool StreamOpen { get; }


		#region Flush

		bool FlushAutomatically { get; }

		void Flush();

		Task FlushAsync();

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