using System;
using System.Collections.Generic;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Editor.Window;
using P = System.IO.Path;
using UnityEngine;

namespace DialogueSystem.Data
{
	public sealed class NodeSaveData : ElementSaveData
	{
		public NodeSaveData Next;
		public List<ChoiceSaveData> Choices = new();
		public GroupSaveData GroupSaveData;

		protected override string Path => CreatePath(Name);
		protected override string PreviousPath => CreatePath(previousName);
		protected override string CreatePath(string filename) => P.Combine(DialogueGraphView.C.GraphPath, $"{(GroupSaveData == null ? "Ungrouped" : $"Groups/{GroupSaveData.Name}")}", $"{(String.IsNullOrWhiteSpace(filename) ? Id : filename)}.asset").ReplaceSlash();

		public static NodeSaveData Create(Vector2 position) => ElementSaveData.Create<NodeSaveData>(position);
	}
}