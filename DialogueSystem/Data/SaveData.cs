using System;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Data
{
	///Represents ScriptableObjects that are saved to .asset files and attached to graph elements.
	[Serializable]
	public abstract class SaveData : ScriptableObject
	{
		[HideInInspector]
		public string Id = Guid.NewGuid().ToString();

		public abstract string Name { get; set; }

		[HideInInspector]
		public Vector2 Position;

		protected string path, folderPath, previousName, previousPath;

		protected static T Create<T>(Vector2 position) where T : SaveData
		{
			var saveData = CreateInstance<T>();
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