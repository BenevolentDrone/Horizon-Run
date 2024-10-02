using System.Collections.Generic;

namespace HereticalSolutions.Hierarchy
{
	public interface IReadOnlyHierarchyNode<TContents>
	{
		TContents Contents { get; }
		
		bool IsRoot { get; }

		bool IsLeaf { get; }

		IReadOnlyHierarchyNode<TContents> Parent { get; }

		IEnumerable<IReadOnlyHierarchyNode<TContents>> Children { get; }

		int ChildCount { get; }
	}
}