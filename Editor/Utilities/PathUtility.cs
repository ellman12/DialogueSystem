using System.IO;

namespace DialogueSystem.Editor.Utilities
{
	///Various DialogueSystem utility methods for paths.
	public static class PathUtility
    {
        ///Combines paths and returns a path with only '/' as the separators.
        public static string Combine(params string[] paths) => Path.Combine(paths).ReplaceSlash();
        
		///Replaces all '\' path separators with '/'.
		public static string ReplaceSlash(this string path) => path.Replace('\\', '/');
		
		///Takes an absolute path and returns it relative to Assets/.
		public static string GetRelativePath(string fullPath)
		{
			string path = fullPath.ReplaceSlash().Replace(Constants.ProjectRoot, "");
			
			//Remove pesky / at the start, which breaks AssetDatabase.CreateAsset().
			if (path.StartsWith('/'))
				path = path[1..];

			return path;
		}
	}
}