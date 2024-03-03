using System.IO;
using DialogueSystem.Editor.Utilities;
using UnityEditor;

namespace DialogueSystem.Editor.Window
{
	public sealed class DialogueGraphWindow : EditorWindow
	{
		[MenuItem("Window/Dialogue Graph")]
		public static void Open() => GetWindow<DialogueGraphWindow>("Dialogue Graph");

		//TODO: delete this later
		[MenuItem("DS/Clear &c")]
		public static void Clear(MenuCommand menuCommand)
		{
			Directory.Delete("Assets/DialogueSystem/Graphs", true);
			Directory.CreateDirectory("Assets/DialogueSystem/Graphs");
			AssetDatabase.Refresh();
		}
		
		private void OnEnable()
		{
			rootVisualElement.AddStyleSheet("Constants");
			rootVisualElement.Add(new DialogueGraphView(this));
			rootVisualElement.Add(new DialogueGraphToolbar());
		}
	}
}