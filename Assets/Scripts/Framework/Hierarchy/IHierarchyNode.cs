namespace HereticalSolutions.Hierarchy
{
	public interface IHierarchyNode<TContents>
		: IReadOnlyHierarchyNode<TContents>
	{
		new TContents Contents { get; set; }
		
		void SetParent(
			IReadOnlyHierarchyNode<TContents> parent,
			bool addToParentsChildren = true);

		void RemoveParent(bool removeFromParentsChildren = true);

		void AddChild(
			IReadOnlyHierarchyNode<TContents> child,
			bool setAsChildsParent = true);

		void RemoveChild(
			IReadOnlyHierarchyNode<TContents> child,
			bool removeFromChildsParent = true);

		void RemoveAllChildren(bool removeFromChildrensParent = true);
	}
}