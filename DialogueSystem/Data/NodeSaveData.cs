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

		[SerializeField]
		private new string name = "";
		public string Name
		{
			get => name;
			set
			{
				previousName = name;
				previousPath = Path.Combine(DialogueGraphView.C.GraphPath, GroupSaveData == null ? "Ungrouped" : $"Groups/{GroupSaveData.Name}", $"{previousName}.asset").ReplaceSlash();

				name = String.IsNullOrWhiteSpace(value) ? Id : value.Trim();
				folderPath = Path.Combine(DialogueGraphView.C.GraphPath, GroupSaveData == null ? "Ungrouped" : $"Groups/{GroupSaveData.Name}").ReplaceSlash();
				path = Path.Combine(folderPath, $"{name}.asset").ReplaceSlash();
			}
		}

		public string Text = "Text";
		private GroupSaveData groupSaveData;
		public GroupSaveData GroupSaveData
		{
			get => groupSaveData;
			set
			{
				groupSaveData = value;
				previousName = name;
				previousPath = path;
				folderPath = Path.Combine(DialogueGraphView.C.GraphPath, GroupSaveData == null ? "Ungrouped" : $"Groups/{GroupSaveData.Name}").ReplaceSlash();
				path = Path.Combine(folderPath, $"{name}.asset").ReplaceSlash();
			}
		}

		public NodeSaveData Next;
		public List<ChoiceSaveData> Choices = new();

		[SerializeField, HideInInspector]
		public Vector2 Position;

		private string path, folderPath, previousName, previousPath;

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
			TryRename();

			AssetDatabase.SaveAssetIfDirty(this);
			EditorUtility.ClearDirty(this);
		}

		public void Delete() => AssetDatabase.DeleteAsset(path);

		private void TryRename()
		{
			if (!File.Exists(path) && !File.Exists(previousPath))
				AssetDatabase.CreateAsset(this, path);
			else if (previousName != Name)
				AssetDatabase.RenameAsset(previousPath, Name);
			else if (previousPath != path)
				AssetDatabase.MoveAsset(previousPath, path);
		}
	}
}