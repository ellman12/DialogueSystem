using System;
using System.IO;
using P = System.IO.Path;
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

		[HideInInspector]
		public Vector2 Position;

		protected string previousName = "";

		protected abstract string Path { get; }
		protected abstract string PreviousPath { get; }
		protected abstract string CreatePath(string filename);

		protected static T Create<T>(Vector2 position) where T : ElementSaveData
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