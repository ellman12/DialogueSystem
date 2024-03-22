using UnityEngine;

namespace DialogueSystem.Data
{
	public sealed class GroupSaveData : ElementSaveData
	{
		public static GroupSaveData Create(Vector2 position) => ElementSaveData.Create<GroupSaveData>(position);
	}
}