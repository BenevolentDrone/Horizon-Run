using System;

namespace HereticalSolutions.Metadata
{
	public interface IStronglyTypedMetadata
	{
		bool Has<TMetadata>();

		TMetadata Get<TMetadata>();
	}
}