using System;
using System.IO;
using DialogueSystem.Editor.Utilities;
using P = System.IO.Path;
using DialogueSystem.Editor.Window;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Data
{
	[Serializable]
	public abstract class ElementSaveData : ScriptableObject
	{
		[HideInInspector]
		public string Id = Guid.NewGuid().ToString();

		public string Name = "";
		public string Text = "Text";

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

		private protected string previousName = "";

		private protected string Path => P.Combine(DialogueGraphView.C.GraphPath, $"{(String.IsNullOrWhiteSpace(Name) ? Id : Name)}.asset").ReplaceSlash();
		private protected string PreviousPath => P.Combine(DialogueGraphView.C.GraphPath, $"{(String.IsNullOrWhiteSpace(previousName) ? Id : previousName)}.asset").ReplaceSlash();

		private protected static T Create<T>(Vector2 position) where T : ElementSaveData
		{
			var saveData = CreateInstance<T>();
			saveData.Position = position;
			return saveData;
		}

		public void FocusIn() => previousName = Name;

		public void FocusOut() => Save();

		public void Delete() => AssetDatabase.DeleteAsset(Path);

		public void Save()
		{
			EditorUtility.SetDirty(this);
			Name = Name.Trim();
			TryRename();
			
			AssetDatabase.SaveAssetIfDirty(this);
			EditorUtility.ClearDirty(this);
		}
		
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