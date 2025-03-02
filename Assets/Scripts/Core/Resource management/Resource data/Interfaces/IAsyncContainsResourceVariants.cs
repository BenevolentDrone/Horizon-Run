using System.Threading.Tasks;

namespace HereticalSolutions.ResourceManagement
{
	public interface IAsyncContainsResourceVariants
	{
		Task<IResourceVariantData> GetDefaultVariantWhenAvailable();

		Task<IResourceVariantData> GetVariantWhenAvailable(int variantIDHash);

		Task<IResourceVariantData> GetVariantWhenAvailable(string variantID);
	}
}