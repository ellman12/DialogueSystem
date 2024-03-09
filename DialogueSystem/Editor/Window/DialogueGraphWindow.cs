using System.IO;
using DialogueSystem.Editor.Extensions;
using UnityEditor;

namespace DialogueSystem.Editor.Window
{
	public sealed class DialogueGraphWindow : EditorWindow
	{
		[MenuItem("Window/Dialogue Graph")]
		public static void Open() => GetWindow<DialogueGraphWindow>("Dialogue Graph");

		private DialogueGraphView graphView;

		//TODO: delete this later
		[MenuItem("DS/Clear &c")]
		public static void Clear(MenuCommand menuCommand)
		{
			Directory.Delete("Assets/DialogueSystem/Graphs", true);
			Directory.CreateDirectory("Assets/DialogueSystem/Graphs");
			AssetDatabase.Refresh();
		}

		public void LoadGraph(string path)
		{
			graphView.GraphName = Path.GetFileName(path);
			graphView.GraphPath = path[1..]; //Remove pesky / at the start.
			graphView.Show();
		}

		public void CloseGraph()
		{
			graphView.Hide();
		}
		
		private void OnEnable()
		{
			rootVisualElement.AddStyleSheet("Constants");

			graphView = new DialogueGraphView(this);
			graphView.Hide();
			rootVisualElement.Add(graphView);
			
			rootVisualElement.Add(new DialogueGraphToolbar(this));
		}
	}
}