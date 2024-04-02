using System;
using System.IO;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Editor.Window;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Data
{
	public sealed class GroupSaveData : SaveData
	{
		public override string Name
		{
			get => name;
			set
			{
				previousName = name;
				previousFolderPath = Path.Combine(DialogueGraphView.C.GraphPath, "Groups", previousName).ReplaceSlash();
				previousPath = Path.Combine(previousFolderPath, $"{previousName}.asset").ReplaceSlash();

				string newName = String.IsNullOrWhiteSpace(value) ? Id : value.Trim();
				folderPath = Path.Combine(DialogueGraphView.C.GraphPath, "Groups", newName).ReplaceSlash();
				path = Path.Combine(folderPath, $"{newName}.asset").ReplaceSlash();

				if (!File.Exists(path) && !File.Exists(previousPath))
				{
					Directory.CreateDirectory(folderPath);
					AssetDatabase.CreateAsset(this, path);
				}
				else if (previousPath != path)
				{
					AssetDatabase.RenameAsset(previousPath, newName);
					AssetDatabase.MoveAsset(previousFolderPath, folderPath);
				}
				
				AssetDatabase.Refresh();
			}
		}

		private string previousFolderPath;

		public static GroupSaveData Create(Vector2 position) => SaveData.Create<GroupSaveData>(position);

		public override void Delete()
		{
			AssetDatabase.DeleteAsset(path);
			AssetDatabase.DeleteAsset(folderPath);
		}
	}
}