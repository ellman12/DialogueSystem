using DialogueSystem.Editor.Utilities;
using DialogueSystem.Editor.Window;
using P = System.IO.Path;
using UnityEngine;

namespace DialogueSystem.Data
{
	public sealed class GroupSaveData : ElementSaveData
	{
		protected override string Path => CreatePath(Name);
		protected override string PreviousPath => CreatePath(previousName);
		protected override string CreatePath(string filename) => P.Combine(DialogueGraphView.C.GraphPath, "Groups", Name, $"{filename}.asset").ReplaceSlash();
		
		public static GroupSaveData Create(Vector2 position) => ElementSaveData.Create<GroupSaveData>(position);
	}
}