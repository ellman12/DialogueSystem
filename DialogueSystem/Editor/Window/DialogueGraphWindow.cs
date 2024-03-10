using System;
using System.IO;
using DialogueSystem.Editor.Extensions;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Editor.Window
{
	public sealed class DialogueGraphWindow : EditorWindow
	{
		[MenuItem("Window/Dialogue Graph")]
		public static void Open() => GetWindow<DialogueGraphWindow>("Dialogue Graph");
		
		private static readonly string ProjectRoot = Environment.CurrentDirectory.Replace('\\', '/');

		private DialogueGraphView graphView;

		//TODO: delete this later
		[MenuItem("DS/Clear &c")]
		public static void Clear(MenuCommand menuCommand)
		{
			Directory.Delete("Assets/DialogueSystem/Graphs", true);
			Directory.CreateDirectory("Assets/DialogueSystem/Graphs");
			AssetDatabase.Refresh();
		}

		public void LoadGraph(string fullPath)
		{
			string graphName = Path.GetFileName(fullPath);
			titleContent = new GUIContent($"{graphName}");
			
			graphView.GraphName = graphName;
			graphView.GraphPath = fullPath.Replace(ProjectRoot, "")[1..]; //Remove pesky / at the start, which breaks AssetDatabase.CreateAsset().
			graphView.Show();
		}

		public void CloseGraph()
		{
			graphView.GraphName = graphView.GraphPath = "";
			titleContent = new GUIContent("Dialogue Graph");
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