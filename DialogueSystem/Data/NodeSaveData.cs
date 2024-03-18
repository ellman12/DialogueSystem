using System;
using System.Collections.Generic;
using System.IO;
using DialogueSystem.Editor.Elements;
using DialogueSystem.Editor.Window;
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
		
		private string Path => P.Combine(DialogueGraphView.C.GraphPath, $"{(String.IsNullOrWhiteSpace(Name) ? Id : Name)}.asset").Replace('\\', '/');

		[NonSerialized]
		private string previousName = "";
		private string PreviousPath => P.Combine(DialogueGraphView.C.GraphPath, $"{(String.IsNullOrWhiteSpace(previousName) ? Id : previousName)}.asset").Replace('\\', '/');

		public static NodeSaveData Create(Vector2 position)
		{
			var saveData = CreateInstance<NodeSaveData>();
			saveData.Position = position;
			return saveData;
		}

		public void FocusIn() => previousName = Name;

		public void FocusOut() => Save();

		public void Save()
		{
			EditorUtility.SetDirty(this);
			Name = Name.Trim();
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