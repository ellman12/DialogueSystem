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

		///Loads all .asset files at the path.
		public static T[] GetAssetsAtPath<T>(string path) where T : ScriptableObject
		{
			string[] files = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);
			return files.Select(AssetDatabase.LoadAssetAtPath<T>).ToArray();
		}

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
		}
	}
}