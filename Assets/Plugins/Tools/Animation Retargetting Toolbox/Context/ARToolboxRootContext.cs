using HereticalSolutions.Metadata;

namespace HereticalSolutions.Tools.AnimationRetargettingToolbox
{
	public class ARToolboxRootContext
		: IARToolboxContext
	{
		private readonly IWeaklyTypedMetadata metadata;

		public ARToolboxRootContext(
			IWeaklyTypedMetadata metadata)
		{
			this.metadata = metadata;
		}

		#region IARToolboxContext

		public double DeltaTime { get; set; }

		public bool Has(string key)
		{
			return metadata.Has(key);
		}

		public bool TryGet<TValue>(
			string key,
			out TValue value)
		{
			if (!metadata.TryGet(
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
			metadata.AddOrUpdate(
				key,
				value);
		}

		public void TryRemove(
			string key)
		{
			metadata.TryRemove(key);
		}

		public void Clear()
		{
			metadata.Clear();
		}

		#endregion
	}
}