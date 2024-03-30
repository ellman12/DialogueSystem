using System;
using System.Collections.Generic;
using System.IO;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Editor.Window;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Data
{
	public sealed class NodeSaveData : SaveData
	{
		//TODO: try to reduce this repetition
		public override string Name
		{
			get => name;
			set
			{
				previousName = name;
				previousPath = Path.Combine(DialogueGraphView.C.GraphPath, Group == null ? "Ungrouped" : $"Groups/{Group.Name}", $"{previousName}.asset").ReplaceSlash();

				//Setting name directly does not work. RenameAsset() sets name for us.
				string newName = String.IsNullOrWhiteSpace(value) ? Id : value.Trim();
				folderPath = Path.Combine(DialogueGraphView.C.GraphPath, Group == null ? "Ungrouped" : $"Groups/{Group.Name}").ReplaceSlash();
				path = Path.Combine(folderPath, $"{newName}.asset").ReplaceSlash();

				if (!File.Exists(path) && !File.Exists(previousPath))
					AssetDatabase.CreateAsset(this, path);
				else if (previousName != Name)
					AssetDatabase.RenameAsset(previousPath, Name);
				else if (previousPath != path)
					AssetDatabase.MoveAsset(previousPath, path);
			}
		}

		public string Text = "Text";

		public NodeSaveData Next;
		public List<ChoiceSaveData> Choices = new();

		[SerializeField]
		private GroupSaveData group;
		public GroupSaveData Group
		{
			get => group;
			set
			{
				group = value;
				previousName = name;
				previousPath = path;
				folderPath = Path.Combine(DialogueGraphView.C.GraphPath, Group == null ? "Ungrouped" : $"Groups/{Group.Name}").ReplaceSlash();
				path = Path.Combine(folderPath, $"{name}.asset").ReplaceSlash();
				
				AssetDatabase.MoveAsset(previousPath, path);
			}
		}

		public static NodeSaveData Create(Vector2 position) => SaveData.Create<NodeSaveData>(position);
	}
}