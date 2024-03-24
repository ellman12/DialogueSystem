using System;
using System.IO;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Editor.Window;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Data
{
	[Serializable]
	public sealed class GroupSaveData : ScriptableObject
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
				previousPath = Path.Combine(DialogueGraphView.C.GraphPath, "Groups", previousName, $"{previousName}.asset").ReplaceSlash();

				name = String.IsNullOrWhiteSpace(value) ? Id : value.Trim();
				folderPath = Path.Combine(DialogueGraphView.C.GraphPath, "Groups", name).ReplaceSlash();
				path = Path.Combine(folderPath, $"{name}.asset").ReplaceSlash();
				
				Directory.CreateDirectory(folderPath);
				AssetDatabase.Refresh();
			}
		}

		[SerializeField, HideInInspector]
		private Vector2 position;
		public Vector2 Position
		{
			get => position;
			set
			{
				position = value;
				Save();
			}
		}

		private string path, folderPath, previousName, previousPath;

		public static GroupSaveData Create(Vector2 position)
		{
			var saveData = CreateInstance<GroupSaveData>();
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
			else if (previousPath != path)
				AssetDatabase.RenameAsset(previousPath, Name);
		}
	}
}