namespace DialogueSystem.Editor.Utilities
{
	///Various DialogueSystem utility methods for paths.
	public static class PathUtility
	{
		///Takes an absolute path and returns it relative to Assets/.
		public static string GetRelativePath(string fullPath)
		{
			string path = fullPath.Replace('\\', '/').Replace(Constants.ProjectRoot, "");
			
			//Remove pesky / at the start, which breaks AssetDatabase.CreateAsset().
			if (path.StartsWith('/'))
				path = path[1..];

			return path;
		}
	}
}