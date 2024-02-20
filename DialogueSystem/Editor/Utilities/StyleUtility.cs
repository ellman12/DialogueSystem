using UnityEditor;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Utilities
{
    public static class StyleUtility
    {
        ///Adds stylesheet(s) to this element.
        public static void AddStyleSheets(this VisualElement element, params string[] styleSheetNames)
        {
            foreach (string name in styleSheetNames)
                element.styleSheets.Add((StyleSheet) EditorGUIUtility.Load(name));
        }
        
        ///Adds class name(s) from a .uss file to this element.
        public static void AddClasses(this VisualElement element, params string[] classNames)
        {
            foreach (string name in classNames)
                element.AddToClassList(name);
        }
    }
}