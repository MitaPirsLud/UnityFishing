using UnityEngine;
using UnityEditor;
using System.Reflection;

public static class RealisticComponentSearch 
{
	// filter will be the search field.
	//ALL = 0;
	//NAME = 1;
	//TYPE = 2;
	static SearchableEditorWindow hierarchy;
	public static void SetSearchFilter(string filter, int filterMode)
	{

		SearchableEditorWindow[] windows = (SearchableEditorWindow[])Resources.FindObjectsOfTypeAll (typeof(SearchableEditorWindow));
		foreach (SearchableEditorWindow window in windows)
		{
			if(window.GetType().ToString() == "UnityEditor.SceneHierarchyWindow") 
			{
				hierarchy = window;
				break;
			}
		}
		if (hierarchy == null)
			return;
		MethodInfo setSearchType = typeof(SearchableEditorWindow).GetMethod("SetSearchFilter", BindingFlags.NonPublic | BindingFlags.Instance);     

		object[] parameters = new object[]{filter, filterMode, false};
		setSearchType.Invoke(hierarchy, parameters);
	}
}
