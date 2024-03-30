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
				previousPath = Path.Combine(DialogueGraphView.C.GraphPath, $"{previousName}.asset").ReplaceSlash();

				//Setting name directly does not work. RenameAsset() sets name for us.
				string newName = String.IsNullOrWhiteSpace(value) ? Id : value.Trim();
				path = Path.Combine(DialogueGraphView.C.GraphPath, $"{newName}.asset").ReplaceSlash();

				if (!File.Exists(path) && !File.Exists(previousPath))
					AssetDatabase.CreateAsset(this, path);
				else if (previousPath != path)
					AssetDatabase.RenameAsset(previousPath, newName);
			}
		}

		public string Text = "Text";
		public GroupSaveData Group;

		public NodeSaveData Next;
		public List<ChoiceSaveData> Choices = new();

		public static NodeSaveData Create(Vector2 position) => SaveData.Create<NodeSaveData>(position);
	}
}