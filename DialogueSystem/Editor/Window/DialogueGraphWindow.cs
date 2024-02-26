using DialogueSystem.Editor.Utilities;
using UnityEditor;

namespace DialogueSystem.Editor.Window
{
	public sealed class DialogueGraphWindow : EditorWindow
	{
		[MenuItem("Window/Dialogue Graph")]
		public static void Open() => GetWindow<DialogueGraphWindow>("Dialogue Graph");

		private void OnEnable()
		{
			rootVisualElement.AddStyleSheet("Constants");
			rootVisualElement.Add(new DialogueGraphView(this));
			rootVisualElement.Add(new DialogueGraphToolbar());
		}
	}
}