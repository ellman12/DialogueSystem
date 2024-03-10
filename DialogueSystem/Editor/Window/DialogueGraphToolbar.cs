using System;
using System.IO;
using DialogueSystem.Editor.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Window
{
	public sealed class DialogueGraphToolbar : Toolbar
	{
		private readonly DialogueGraphWindow window;
		private readonly Label error = new();

		private const string GraphsRoot = "Assets/DialogueSystem/Graphs";

		public DialogueGraphToolbar(DialogueGraphWindow window)
		{
			this.window = window;
			this.AddStyleSheet("Toolbar");

			this.AddButton("Close", CloseGraph);
			this.AddButton("Load", TryLoadGraph);
			this.AddButton("Create", CreateGraph);

			error.style.color = Color.red;
			Add(error);
		}
		
		private void CloseGraph()
		{
			window.CloseGraph();
		}

		private void TryLoadGraph()
		{
			string fullPath = EditorUtility.OpenFolderPanel("Choose Folder", GraphsRoot, "");

			if (String.IsNullOrWhiteSpace(fullPath))
				return;

			if (ValidGraph(fullPath))
			{
				window.LoadGraph(fullPath);
				error.text = "";
			}
			else
			{
				error.text = "Folder is invalid!";
			}
		}

		private static bool ValidGraph(string fullPath)
		{
			string ungroupedPath = Path.Combine(fullPath, "Ungrouped");
			string groupsPath = Path.Combine(fullPath, "Groups");
			return Directory.Exists(fullPath) && Directory.Exists(ungroupedPath) && Directory.Exists(groupsPath);
		}

		private void CreateGraph()
		{
			string fullPath = EditorUtility.SaveFolderPanel("Choose Folder", GraphsRoot, "");

			if (String.IsNullOrWhiteSpace(fullPath))
				return;

			window.LoadGraph(fullPath);

			Directory.CreateDirectory(fullPath);
			Directory.CreateDirectory(Path.Combine(fullPath, "Ungrouped"));
			Directory.CreateDirectory(Path.Combine(fullPath, "Groups"));
			AssetDatabase.Refresh();
		}
	}
}