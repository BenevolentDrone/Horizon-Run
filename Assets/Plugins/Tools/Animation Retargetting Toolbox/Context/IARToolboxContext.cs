using HereticalSolutions.Repositories;

namespace HereticalSolutions.Tools.AnimationRetargettingToolbox
{
	public interface IARToolboxContext
	{
		double DeltaTime { get; }

		bool Has(string key);

		bool TryGet<TValue>(
			string key,
			out TValue value);

		void AddOrUpdate<TValue>(
			string key,
			TValue value);

		void TryRemove(
			string key);

		void Clear();
	}
}