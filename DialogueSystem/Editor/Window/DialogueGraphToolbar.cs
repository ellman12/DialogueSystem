﻿using System;
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
		private static readonly string ProjectRoot = Environment.CurrentDirectory.Replace('\\', '/');
		private static readonly string GraphsRootFull = Path.Combine(Environment.CurrentDirectory, GraphsRoot).Replace('\\', '/');

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
			string path = EditorUtility.OpenFolderPanel("Choose Folder", GraphsRoot, "");

			if (String.IsNullOrWhiteSpace(path))
				return;

			if (ValidGraph(path))
			{
				window.LoadGraph(path.Replace(ProjectRoot, ""));
				error.text = "";
			}
			else
			{
				error.text = "Folder is invalid!";
			}
		}

		private static bool ValidGraph(string path)
		{
			string ungroupedPath = Path.Combine(path, "Ungrouped");
			string groupsPath = Path.Combine(path, "Groups");
			return Directory.Exists(path) && Directory.Exists(ungroupedPath) && Directory.Exists(groupsPath);
		}

		private void CreateGraph()
		{
			string path = EditorUtility.SaveFolderPanel("Choose Folder", GraphsRoot, "");

			if (String.IsNullOrWhiteSpace(path))
				return;

			window.LoadGraph(path.Replace(ProjectRoot, ""));

			Directory.CreateDirectory(path);
			Directory.CreateDirectory(Path.Combine(path, "Ungrouped"));
			Directory.CreateDirectory(Path.Combine(path, "Groups"));
			AssetDatabase.Refresh();
		}
	}
}