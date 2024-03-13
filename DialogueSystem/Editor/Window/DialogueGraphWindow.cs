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
		public static void Open() => GetWindow<DialogueGraphWindow>("Dialogue Graph");

		public const string GraphsRoot = "Assets/DialogueSystem/Graphs";

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

		///Takes an absolute path and returns it relative to Assets/.
		public static string GetRelativePath(string fullPath) => fullPath.Replace(ProjectRoot, "")[1..]; //Remove pesky / at the start, which breaks AssetDatabase.CreateAsset().

		///Loads all .asset files at the path.
		public static T[] GetAssetsAtPath<T>(string path) where T : ScriptableObject
		{
			string[] files = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);
			return files.Select(AssetDatabase.LoadAssetAtPath<T>).ToArray();
		}

		private void OnEnable()
		{
			rootVisualElement.AddStyleSheet("Constants");

			graphView = new DialogueGraphView(this);
			graphView.Hide();
			rootVisualElement.Add(graphView);

			rootVisualElement.Add(new DialogueGraphToolbar(graphView));

            Directory.CreateDirectory(GraphsRoot);
            AssetDatabase.Refresh();
        }
	}
}