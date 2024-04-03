using System;
using System.IO;
using System.Linq;
using DialogueSystem.Editor.Extensions;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Editor.Window
{
	public sealed class DialogueGraphWindow : EditorWindow
	{
		[MenuItem("Window/Dialogue Graph")]
		public static void Open() => CreateWindow<DialogueGraphWindow>("Dialogue Graph");

		private DialogueGraphView graphView;
		public static DialogueGraphView GraphView => C.graphView;

		private DialogueGraphToolbar toolbar; 
		public static DialogueGraphToolbar Toolbar => C.toolbar;

		public static DialogueGraphWindow C;

		//TODO: delete this later
		[MenuItem("DS/Clear &c")]
		public static void Clear(MenuCommand menuCommand)
		{
			Directory.Delete("Assets/DialogueSystem/Graphs", true);
			Directory.CreateDirectory("Assets/DialogueSystem/Graphs");
			AssetDatabase.Refresh();
		}

		public void SetTitle(string newTitle) => titleContent.text = newTitle;

		private void OnFocus() => C = this;

		private void OnEnable()
		{
			C = this;
            
			rootVisualElement.AddStyleSheet("Constants");

			graphView = new DialogueGraphView();
			graphView.Hide();
			rootVisualElement.Add(graphView);

			toolbar = new DialogueGraphToolbar();
			rootVisualElement.Add(toolbar);

			Directory.CreateDirectory(Constants.GraphsRoot);
			AssetDatabase.Refresh();

			//TODO: remove this later
			// string path = Path.Combine(Environment.CurrentDirectory, Constants.GraphsRoot, Guid.NewGuid().ToString()).Replace('\\', '/');
			// graphView.CreateGraph(path);
		}
	}
}