using System;
using System.Reflection;
using System.Collections.Generic;

using HereticalSolutions.Entities;

using UnityEditor.Experimental.GraphView;

using UnityEngine;

namespace HereticalSolutions.Entities.Editor
{
	/// <summary>
	/// Provides a list of component types for searching.
	/// </summary>
	public class ComponentTypesProvider : ScriptableObject, ISearchWindowProvider
	{
		private List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
		private List<string> groupPaths = new List<string>();

		/// <summary>
		/// Delegate for when a component is selected.
		/// </summary>
		public Action<Type> OnComponentSelected { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentTypesProvider"/> class.
		/// </summary>
		public ComponentTypesProvider()
		{
			UpdateList();
		}

		/// <summary>
		/// Creates the search tree for the search window context.
		/// </summary>
		/// <param name="context">The search window context.</param>
		/// <returns>The search tree for the search window context.</returns>
		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
		{
			return searchList;
		}

		/// <summary>
		/// Handles the selection of a search tree entry.
		/// </summary>
		/// <param name="SearchTreeEntry">The selected search tree entry.</param>
		/// <param name="context">The search window context.</param>
		/// <returns>True if the entry was selected, false otherwise.</returns>
		public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
		{
			OnComponentSelected?.Invoke((Type)SearchTreeEntry.userData);

			return true;
		}

		/// <summary>
		/// Updates the search list.
		/// </summary>
		private void UpdateList()
		{
			searchList.Clear();
			groupPaths.Clear();

			searchList.Add(
				new SearchTreeGroupEntry(
					new GUIContent(
						"Components"),
					0));

			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assembly.GetTypes())
				{
					var componentAttribute = type.GetCustomAttribute<ComponentAttribute>(false);

					if (componentAttribute == null)
						continue;

					string[] pathParts = componentAttribute.Path.Split('/');

					string currentGroupPath = "";

					for (int i = 0; i < pathParts.Length; i++)
					{
						string currentPathPart = pathParts[i];

						currentGroupPath = $"{currentGroupPath}/{currentPathPart}";

						if (groupPaths.Contains(currentGroupPath))
							continue;

						groupPaths.Add(currentGroupPath);

						searchList.Add(
							new SearchTreeGroupEntry(
								new GUIContent(
									currentPathPart),
								i + 1));
					}

					SearchTreeEntry componentEntry = new SearchTreeEntry(
						new GUIContent(
							type.Name));

					componentEntry.level = pathParts.Length + 1;

					componentEntry.userData = type;

					searchList.Add(componentEntry);
				}
			}

			//searchList.Sort();
		}
	}
}