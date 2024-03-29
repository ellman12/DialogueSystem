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

		public string Name = "";
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

		public string FolderContaining { get; set; } = "";
		
		private string Path => P.Combine(FolderContaining, $"{(String.IsNullOrWhiteSpace(Name) ? Id : Name)}.asset").Replace('\\', '/');

		[NonSerialized]
		private string previousName = "";
		private string PreviousPath => P.Combine(FolderContaining, $"{(String.IsNullOrWhiteSpace(previousName) ? Id : previousName)}.asset").Replace('\\', '/');

		public static NodeSaveData Create(string folderContaining, Vector2 position)
		{
			var saveData = CreateInstance<NodeSaveData>();
			saveData.FolderContaining = folderContaining;
			saveData.Position = position;
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
			if (!File.Exists(Path) && !File.Exists(PreviousPath))
				AssetDatabase.CreateAsset(this, Path);
			else if (PreviousPath != Path)
				AssetDatabase.RenameAsset(PreviousPath, Name);
			
			previousName = Name;
		}
	}
}