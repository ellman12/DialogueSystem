using System;
using System.Collections.Generic;
using System.IO;
using DialogueSystem.Editor.Elements;
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
				previousPath = Path.Combine(DialogueGraphView.C.GraphPath, $"{previousName}.asset").ReplaceSlash();

				name = String.IsNullOrWhiteSpace(value) ? Id : value.Trim();
				path = Path.Combine(DialogueGraphView.C.GraphPath, $"{name}.asset").ReplaceSlash();
			}
		}

		public string Text = "Text";
		public DialogueGroup Group;

		public NodeSaveData Next;
		public List<ChoiceSaveData> Choices = new();

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
		
		private string path, previousName, previousPath;

		public static NodeSaveData Create(Vector2 position)
		{
			var saveData = CreateInstance<NodeSaveData>();
			saveData.Name = saveData.Id;
			saveData.Position = position;
			saveData.Save();
			return saveData;
		}

		public void FocusIn() => previousName = Name;

		public void FocusOut() => Save();

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