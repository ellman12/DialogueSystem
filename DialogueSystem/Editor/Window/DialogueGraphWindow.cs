using System;
using System.IO;
using DialogueSystem.Editor.Extensions;
using UnityEditor;

namespace DialogueSystem.Editor.Window
{
	public sealed class DialogueGraphWindow : EditorWindow
	{
		[MenuItem("Window/Dialogue Graph")]
		public static void Open() => GetWindow<DialogueGraphWindow>("Dialogue Graph");
		
		public static readonly string ProjectRoot = Environment.CurrentDirectory.Replace('\\', '/');

		private DialogueGraphView graphView;

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

			graphView = new DialogueGraphView(this);
			graphView.Hide();
			rootVisualElement.Add(graphView);
			
			rootVisualElement.Add(new DialogueGraphToolbar(graphView));
		}
	}
}