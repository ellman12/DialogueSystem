using System.Collections.Generic;
using P = System.IO.Path;
using UnityEngine;

namespace DialogueSystem.Data
{
	public sealed class NodeSaveData : ElementSaveData
	{
		public NodeSaveData Next;
		public List<ChoiceSaveData> Choices = new();
		public GroupSaveData GroupSaveData;

		public static NodeSaveData Create(Vector2 position) => ElementSaveData.Create<NodeSaveData>(position);
	}
}