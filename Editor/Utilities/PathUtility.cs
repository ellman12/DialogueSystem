using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DialogueSystem.Editor.Utilities
{
    ///Various DialogueSystem utility methods for paths.
    public static class PathUtility
    {
        private static readonly HashSet<char> invalid = Path.GetInvalidFileNameChars().ToHashSet();

        ///Combines paths and returns a path with only '/' as the separators.
        public static string Combine(params string[] paths) => Path.Combine(paths).ReplaceSlash();

        ///Returns a path with all the invalid characters removed.
        public static string RemoveInvalidChars(this string path) => new(path.Where(c => !invalid.Contains(c)).ToArray());

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