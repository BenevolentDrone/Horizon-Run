using HereticalSolutions.Repositories;

namespace HereticalSolutions.Tools.AnimationRetargettingToolbox
{
	public class ARToolboxRootContext
		: IARToolboxContext
	{
		private readonly IRepository<string, object> metadataRepository;

		public ARToolboxRootContext(
			IRepository<string, object> metadataRepository)
		{
			this.metadataRepository = metadataRepository;
		}

		#region IARToolboxContext

		public double DeltaTime { get; set; }

		public bool Has(string key)
		{
			return metadataRepository.Has(key);
		}

		public bool TryGet<TValue>(
			string key,
			out TValue value)
		{
			if (!metadataRepository.TryGet(
				key,
				out var result))
			{
				value = default;

				return false;
			}

			switch (result)
			{
				case TValue typedValue:
				{
					value = typedValue;

					return true;
				}
				default:
				{
					value = default;

					return false;
				}
			}
		}

		public void AddOrUpdate<TValue>(
			string key,
			TValue value)
		{
			metadataRepository.AddOrUpdate(
				key,
				value);
		}

		public void TryRemove(
			string key)
		{
			metadataRepository.TryRemove(key);
		}

		public void Clear()
		{
			metadataRepository.Clear();
		}

		#endregion
	}
}