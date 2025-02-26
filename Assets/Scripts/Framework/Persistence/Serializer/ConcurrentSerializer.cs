using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Persistence
{
	public class ConcurrentSerializer
		: ISerializer,
		  IBlockSerializer,
		  IAsyncSerializer,
		  IAsyncBlockSerializer
	{
		private readonly Serializer serializer;

		private readonly SemaphoreSlim semaphoreSlim;

		public ConcurrentSerializer(
			Serializer serializer,
			SemaphoreSlim semaphoreSlim)
		{
			this.serializer = serializer;

			this.semaphoreSlim = semaphoreSlim;
		}

		#region ISerializer

		#region IHasIODestination

		public void EnsureIODestinationExists()
		{
			semaphoreSlim.Wait();

			serializer.EnsureIODestinationExists();

			semaphoreSlim.Release();
		}

		public bool IODestinationExists()
		{
			semaphoreSlim.Wait();

			bool result = serializer.IODestinationExists();

			semaphoreSlim.Release();

			return result;
		}

		public void CreateIODestination()
		{
			semaphoreSlim.Wait();

			serializer.CreateIODestination();

			semaphoreSlim.Release();
		}

		public void EraseIODestination()
		{
			semaphoreSlim.Wait();

			serializer.EraseIODestination();

			semaphoreSlim.Release();
		}

		#endregion

		#region IHasReadWriteControl

		public bool SupportsSimultaneousReadAndWrite
		{
			get
			{
				semaphoreSlim.Wait();

				bool result = serializer.SupportsSimultaneousReadAndWrite;

				semaphoreSlim.Release();

				return result;
			}
		}

		public void InitializeRead()
		{
			semaphoreSlim.Wait();

			serializer.InitializeRead();

			semaphoreSlim.Release();
		}

		public void FinalizeRead()
		{
			semaphoreSlim.Wait();

			serializer.FinalizeRead();

			semaphoreSlim.Release();
		}


		public void InitializeWrite()
		{
			semaphoreSlim.Wait();

			serializer.InitializeWrite();

			semaphoreSlim.Release();
		}

		public void FinalizeWrite()
		{
			semaphoreSlim.Wait();

			serializer.FinalizeWrite();

			semaphoreSlim.Release();
		}


		public void InitializeAppend()
		{
			semaphoreSlim.Wait();

			serializer.InitializeAppend();

			semaphoreSlim.Release();
		}

		public void FinalizeAppend()
		{
			semaphoreSlim.Wait();

			serializer.FinalizeAppend();

			semaphoreSlim.Release();
		}


		public void InitializeReadAndWrite()
		{
			semaphoreSlim.Wait();

			serializer.InitializeReadAndWrite();

			semaphoreSlim.Release();
		}

		public void FinalizeReadAndWrite()
		{
			semaphoreSlim.Wait();

			serializer.FinalizeReadAndWrite();

			semaphoreSlim.Release();
		}

		#endregion

		public IReadOnlySerializerContext Context { get => serializer.Context; }

		#region Serialize

		public bool Serialize<TValue>(
			TValue value)
		{
			semaphoreSlim.Wait();

			bool result = serializer.Serialize(
				value);

			semaphoreSlim.Release();

			return result;
		}

		public bool Serialize(
			Type valueType,
			object valueObject)
		{
			semaphoreSlim.Wait();

			bool result = serializer.Serialize(
				valueType,
				valueObject);

			semaphoreSlim.Release();

			return result;
		}

		#endregion

		#region Deserialize

		public bool Deserialize<TValue>(
			out TValue value)
		{
			semaphoreSlim.Wait();

			bool result = serializer.Deserialize(
				out value);

			semaphoreSlim.Release();

			return result;
		}

		public bool Deserialize(
			Type valueType,
			out object valueObject)
		{
			semaphoreSlim.Wait();

			bool result = serializer.Deserialize(
				valueType,
				out valueObject);

			semaphoreSlim.Release();

			return result;
		}

		#endregion

		#region Populate

		public bool Populate<TValue>(
			TValue value)
		{
			semaphoreSlim.Wait();

			bool result = serializer.Populate(
				value);

			semaphoreSlim.Release();

			return result;
		}

		public bool Populate(
			Type valueType,
			object valueObject)
		{
			semaphoreSlim.Wait();

			bool result = serializer.Populate(
				valueType,
				valueObject);

			semaphoreSlim.Release();

			return result;
		}

		#endregion

		#endregion

		#region IBlockSerializer

		#region Serialize

		public bool SerializeBlock<TValue>(
			TValue value,
			int blockOffset,
			int blockSize)
		{
			semaphoreSlim.Wait();

			bool result = serializer.SerializeBlock(
				value,
				blockOffset,
				blockSize);

			semaphoreSlim.Release();

			return result;
		}

		public bool SerializeBlock(
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize)
		{
			semaphoreSlim.Wait();

			bool result = serializer.SerializeBlock(
				valueType,
				valueObject,
				blockOffset,
				blockSize);

			semaphoreSlim.Release();

			return result;
		}

		#endregion

		#region Deserialize

		public bool DeserializeBlock<TValue>(
			int blockOffset,
			int blockSize,
			out TValue value)
		{
			semaphoreSlim.Wait();

			bool result = serializer.DeserializeBlock(
				blockOffset,
				blockSize,
				out value);

			semaphoreSlim.Release();

			return result;
		}

		public bool DeserializeBlock(
			Type valueType,
			int blockOffset,
			int blockSize,
			out object valueObject)
		{
			semaphoreSlim.Wait();

			bool result = serializer.DeserializeBlock(
				valueType,
				blockOffset,
				blockSize,
				out valueObject);

			semaphoreSlim.Release();

			return result;
		}

		#endregion

		#region Populate

		public bool PopulateBlock<TValue>(
			TValue value,
			int blockOffset,
			int blockSize)
		{
			semaphoreSlim.Wait();

			bool result = serializer.PopulateBlock(
				value,
				blockOffset,
				blockSize);

			semaphoreSlim.Release();

			return result;
		}

		public bool PopulateBlock(
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize)
		{
			semaphoreSlim.Wait();

			bool result = serializer.PopulateBlock(
				valueType,
				valueObject,
				blockOffset,
				blockSize);

			semaphoreSlim.Release();

			return result;
		}

		#endregion

		#endregion

		#region IAsyncSerializer

		#region Serialize

		public async Task<bool> SerializeAsync<TValue>(
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await semaphoreSlim.WaitAsync();

			var asyncResult = await serializer.SerializeAsync(
				value,

				asyncContext);

			semaphoreSlim.Release();

			return asyncResult;
		}

		public async Task<bool> SerializeAsync(
			Type valueType,
			object valueObject,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await semaphoreSlim.WaitAsync();

			var asyncResult = await serializer.SerializeAsync(
				valueType,
				valueObject,

				asyncContext);

			semaphoreSlim.Release();

			return asyncResult;
		}

		#endregion

		#region Deserialize

		public async Task<(bool, TValue)> DeserializeAsync<TValue>(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await semaphoreSlim.WaitAsync();

			var asyncResult = await serializer.DeserializeAsync<TValue>(
				asyncContext);

			semaphoreSlim.Release();

			return asyncResult;
		}

		public async Task<(bool, object)> DeserializeAsync(
			Type valueType,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await semaphoreSlim.WaitAsync();

			var asyncResult = await serializer.DeserializeAsync(
				valueType,

				asyncContext);

			semaphoreSlim.Release();

			return asyncResult;
		}

		#endregion

		#region Populate

		public async Task<bool> PopulateAsync<TValue>(
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await semaphoreSlim.WaitAsync();

			var asyncResult = await serializer.PopulateAsync(
				value,

				asyncContext);

			semaphoreSlim.Release();

			return asyncResult;
		}

		public async Task<bool> PopulateAsync(
			Type valueType,
			object valueObject,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await semaphoreSlim.WaitAsync();

			var asyncResult = await serializer.PopulateAsync(
				valueType,
				valueObject,

				asyncContext);

			semaphoreSlim.Release();

			return asyncResult;
		}

		#endregion

		#endregion

		#region IAsyncBlockSerializer

		#region Serialize

		public async Task<bool> SerializeBlockAsync<TValue>(
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await semaphoreSlim.WaitAsync();

			var asyncResult = await serializer.SerializeBlockAsync(
				value,
				blockOffset,
				blockSize,

				asyncContext);

			semaphoreSlim.Release();

			return asyncResult;
		}

		public async Task<bool> SerializeBlockAsync(
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await semaphoreSlim.WaitAsync();

			var asyncResult = await serializer.SerializeBlockAsync(
				valueType,
				valueObject,
				blockOffset,
				blockSize,

				asyncContext);

			semaphoreSlim.Release();

			return asyncResult;
		}

		#endregion

		#region Deserialize

		public async Task<(bool, TValue)> DeserializeBlockAsync<TValue>(
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await semaphoreSlim.WaitAsync();

			var asyncResult = await serializer.DeserializeBlockAsync<TValue>(
				blockOffset,
				blockSize,

				asyncContext);

			semaphoreSlim.Release();

			return asyncResult;
		}

		public async Task<(bool, object)> DeserializeBlockAsync(
			Type valueType,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await semaphoreSlim.WaitAsync();

			var asyncResult = await serializer.DeserializeBlockAsync(
				valueType,
				blockOffset,
				blockSize,

				asyncContext);

			semaphoreSlim.Release();

			return asyncResult;
		}

		#endregion

		#region Populate

		public async Task<bool> PopulateBlockAsync<TValue>(
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await semaphoreSlim.WaitAsync();

			var asyncResult = await serializer.PopulateBlockAsync(
				value,
				blockOffset,
				blockSize,

				asyncContext);

			semaphoreSlim.Release();

			return asyncResult;
		}

		public async Task<bool> PopulateBlockAsync(
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await semaphoreSlim.WaitAsync();

			var asyncResult = await serializer.PopulateBlockAsync(
				valueType,
				valueObject,
				blockOffset,
				blockSize,

				asyncContext);

			semaphoreSlim.Release();

			return asyncResult;
		}

		#endregion

		#endregion
	}
}