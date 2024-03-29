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
				previousPath = Path.Combine(DialogueGraphView.C.GraphPath, "Groups", previousName, $"{previousName}.asset").ReplaceSlash();

				//TODO: setting name directly like this is probably a problem!
				name = String.IsNullOrWhiteSpace(value) ? Id : value.Trim();
				folderPath = Path.Combine(DialogueGraphView.C.GraphPath, "Groups", name).ReplaceSlash();
				path = Path.Combine(folderPath, $"{name}.asset").ReplaceSlash();

				Directory.CreateDirectory(folderPath);
				AssetDatabase.Refresh();

				if (!File.Exists(path) && !File.Exists(previousPath))
					AssetDatabase.CreateAsset(this, path);
				else if (previousPath != path)
					AssetDatabase.RenameAsset(previousPath, Name);
			}
		}

		private string folderPath;

		public static GroupSaveData Create(Vector2 position) => SaveData.Create<GroupSaveData>(position);
	}
}