namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public delegate bool HasWorldIdentityComponentDelegate<TEntity>(
		TEntity registryEntity);

	public delegate void GetWorldIdentityComponentDelegate<TPrototypeID, TEntity>(
		TEntity registryEntity,
		out TPrototypeID prototypeID,
		out TEntity worldLocalEntity);

	public delegate void SetWorldIdentityComponentDelegate<TPrototypeID, TEntity>(
		TEntity registryEntity,
		TPrototypeID prototypeID,
		TEntity worldLocalEntity);

	public delegate void RemoveWorldIdentityComponentDelegate<TEntity>(
		TEntity registryEntity);
}