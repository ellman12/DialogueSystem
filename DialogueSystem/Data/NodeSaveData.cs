using System;
using System.Collections.Generic;
using System.IO;
using DialogueSystem.Editor.Elements;
using P = System.IO.Path;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Data
{
	[Serializable]
	public sealed class NodeSaveData : ScriptableObject
	{
		[HideInInspector]
		public string Id = Guid.NewGuid().ToString();

		public string Name = DefaultName;
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

		[NonSerialized]
		private string folderContaining = "";
		
		private string Path => P.Combine(folderContaining, $"{Name}.asset").Replace('\\', '/');

		[NonSerialized]
		private string previousName = "";
		private string PreviousPath => P.Combine(folderContaining, $"{previousName}.asset").Replace('\\', '/');

		private const string DefaultName = "New Node";

		public static NodeSaveData Create(string folderContaining, Vector2 position)
		{
			var saveData = CreateInstance<NodeSaveData>();
			saveData.Position = position;
			saveData.folderContaining = folderContaining;
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

		public void Delete() => AssetDatabase.DeleteAsset(Path);

		private void TryRename()
		{
			if (!File.Exists(Path) && !File.Exists(PreviousPath) && Name != DefaultName)
				AssetDatabase.CreateAsset(this, Path);
			else if (PreviousPath != Path)
				AssetDatabase.RenameAsset(PreviousPath, Name);
			
			previousName = Name;
		}
	}
}