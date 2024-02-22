using UnityEditor;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Utilities
{
    public static class StyleUtility
    {
        ///Adds stylesheet(s) to this element.
        public static void AddStyleSheet(this VisualElement element, string styleSheetName)
        {
            element.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>($"Assets/DialogueSystem/Editor/Window/USS/{styleSheetName}"));
        }
        
        ///Adds class name(s) from a .uss file to this element.
        public static void AddClasses(this VisualElement element, params string[] classNames)
        {
            foreach (string name in classNames)
                element.AddToClassList(name);
        }
    }
}