using UnityEditor;

namespace DialogueSystem.Editor.Window
{
	public sealed class DialogueGraphWindow : EditorWindow
	{
		private DialogueGraphView graphView;

		[MenuItem("Window/Dialogue Graph")]
		public static void Open() => GetWindow<DialogueGraphWindow>("Dialogue Graph");

		private void OnEnable()
		{
			graphView = new DialogueGraphView(this);
		}
	}
}