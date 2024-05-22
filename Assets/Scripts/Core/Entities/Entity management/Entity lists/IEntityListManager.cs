namespace HereticalSolutions.Entities
{
	public interface IEntityListManager<TListHandle, TEntityList>
	{
		bool HasList(TListHandle listHandle);

		TEntityList GetList(TListHandle listHandle);

		void CreateList(
			out TListHandle listHandle,
			out TEntityList entityList);

		void RemoveList(TListHandle listHandle);
	}
}