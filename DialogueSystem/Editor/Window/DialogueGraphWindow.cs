using System.IO;
using DialogueSystem.Editor.Extensions;
using UnityEditor;

namespace DialogueSystem.Editor.Window
{
	public sealed class DialogueGraphWindow : EditorWindow
	{
		[MenuItem("Window/Dialogue Graph")]
		public static void Open() => GetWindow<DialogueGraphWindow>("Dialogue Graph");
		
		public string GraphName { get; set; }

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
			GraphName = Path.GetFileName(path);
			graphView.Show();
		}

		public void CloseGraph()
		{
			GraphName = "";
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