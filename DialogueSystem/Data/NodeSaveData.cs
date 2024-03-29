using System;
using System.Collections.Generic;
using System.IO;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Editor.Window;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Data
{
	[Serializable]
	public sealed class NodeSaveData : ScriptableObject
	{
		[HideInInspector]
		public string Id = Guid.NewGuid().ToString();

		public string Name
		{
			get => name;
			set
			{
				previousName = name;
				previousPath = Path.Combine(DialogueGraphView.C.GraphPath, $"{previousName}.asset").ReplaceSlash();

				//Setting name directly will still work, but creates a warning in the console. RenameAsset() sets name for us.
				string newName = String.IsNullOrWhiteSpace(value) ? Id : value.Trim();
				path = Path.Combine(DialogueGraphView.C.GraphPath, $"{newName}.asset").ReplaceSlash();

				if (!File.Exists(path) && !File.Exists(previousPath))
					AssetDatabase.CreateAsset(this, path);
				else if (previousPath != path)
					AssetDatabase.RenameAsset(previousPath, newName);
			}
		}

		public string Text = "Text";
		public GroupSaveData GroupSaveData;

		public NodeSaveData Next;
		public List<ChoiceSaveData> Choices = new();

		[HideInInspector]
		public Vector2 Position;

		private string path, previousName, previousPath;

		public static NodeSaveData Create(Vector2 position)
		{
			var saveData = CreateInstance<NodeSaveData>();
			saveData.Name = saveData.Id;
			saveData.Position = position;
			saveData.Save();
			return saveData;
		}

		public void Save()
		{
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssetIfDirty(this);
			EditorUtility.ClearDirty(this);
		}

		public void Delete() => AssetDatabase.DeleteAsset(path);
	}
}