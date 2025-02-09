using System.IO;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	[SerializationStrategy]
	public class MemoryStreamStrategy
		: AStreamStrategy<MemoryStream>,
		  IStrategyWithFilter
	{
		private readonly byte[] buffer;

		public byte[] Buffer
		{
			get
			{
				if (buffer != null)
					return buffer;

				if (stream != null)
					return stream.GetBuffer();

				return null;
			}
		}

		public MemoryStream MemoryStream => stream;


		private int index = -1;

		public int Index => index;


		private int count = -1;

		public int Count => count;


		public MemoryStreamStrategy(
			ILogger logger,
			
			byte[] buffer = null,
			int index = -1,
			int count = -1)
			: base(
				logger)
		{
			this.buffer = buffer;

			this.index = index;

			this.count = count;
		}

		protected override bool OpenReadStream(
			out MemoryStream dataStream)
		{
			return OpenStream(
				out dataStream);
		}

		protected override bool OpenWriteStream(
			out MemoryStream dataStream)
		{
			return OpenStream(
				out dataStream);
		}

		protected override bool OpenAppendStream(
			out MemoryStream dataStream)
		{
			return OpenStream(
				out dataStream);
		}

		protected override bool OpenReadWriteStream(
			out MemoryStream dataStream)
		{
			return OpenStream(
				out dataStream);
		}

		private bool OpenStream(
			out MemoryStream dataStream)
		{
			if (buffer == null)
			{
				dataStream = new MemoryStream();
			}
			else if (index < 0 && count < 0)
			{
				dataStream = new MemoryStream(
					buffer);
			}
			else
			{
				dataStream = new MemoryStream(
					buffer,
					index,
					count);
			}

			return true;
		}
	}
}