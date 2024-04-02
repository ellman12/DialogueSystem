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
		private readonly Label error = new();

		public DialogueGraphToolbar()
		{
			this.AddStyleSheet("Toolbar");

			this.AddButton("Close", DialogueGraphView.C.CloseGraph);
			this.AddButton("Load", TryLoadGraph);
			this.AddButton("Create", CreateGraph);

			error.style.color = Color.red;
			Add(error);
		}

		private void TryLoadGraph()
		{
			string fullPath = EditorUtility.OpenFolderPanel("Choose Folder", Constants.GraphsRoot, "");

			if (String.IsNullOrWhiteSpace(fullPath))
				return;

			if (ValidGraph(fullPath))
			{
				DialogueGraphView.C.LoadGraph(fullPath);
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
			string fullPath = EditorUtility.SaveFolderPanel("Choose Folder", Constants.GraphsRoot, "");

			if (String.IsNullOrWhiteSpace(fullPath))
				return;

			error.text = "";
			DialogueGraphView.C.CreateGraph(fullPath);
		}
	}
}