using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Editor.Utilities
{
	public static class IOUtility
	{
		///Loads all .asset files of this type and at this path.
		public static T[] GetAssetsAtPath<T>(string path) where T : ScriptableObject
		{
			string[] files = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);
			return files.Select(AssetDatabase.LoadAssetAtPath<T>).Where(asset => asset != null).ToArray();
		}
	}
}