using System;
using System.Collections.Generic;
using System.IO;
using P = System.IO.Path;
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

		public string Name = DefaultName;
		public string Text = "Text";
		public string GroupId = "";

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
		
		private string Path => P.Combine(DialogueGraphView.GraphsRootPath, $"{Name}.asset");

		[NonSerialized]
		private string previousName = "";
		private string PreviousPath => P.Combine(DialogueGraphView.GraphsRootPath, $"{previousName}.asset");

		private const string DefaultName = "New Node";

		public static NodeSaveData Create() => CreateInstance<NodeSaveData>();

		public void FocusIn() => previousName = Name;

		public void FocusOut() => Save();

		public void Save()
		{
			EditorUtility.SetDirty(this);
			TryRename();
			
			AssetDatabase.SaveAssetIfDirty(this);
			EditorUtility.ClearDirty(this);
		}

		private void TryRename()
		{
			if (!File.Exists(Path) && !File.Exists(PreviousPath) && Name != DefaultName)
			{
				AssetDatabase.CreateAsset(this, Path);
			}
			else if (PreviousPath != Path)
			{
				AssetDatabase.RenameAsset(PreviousPath, Name);
				AssetDatabase.Refresh();
			}
			
			previousName = Name;
		}
	}
}