using System;
using DialogueSystem.Data;
using DialogueSystem.Editor.Elements.Interfaces;
using DialogueSystem.Editor.Window;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogueSystem.Editor.Elements
{
	[Serializable]
	public sealed class DialogueGroup : Group, ISaveableElement<GroupSaveData>
	{
		public GroupSaveData SaveData { get; }

		public DialogueGroup(Vector2 position)
		{
			SaveData = GroupSaveData.Create(position);
			title = SaveData.Id;
			SetPosition(new Rect(position, Vector2.zero));
		}

		public void UpdatePosition(Vector2 newPosition)
		{
			SaveData.Position = newPosition;
			SaveData.Save();
		}

		public void Remove()
		{
			DialogueGraphView.C.RemoveElement(this);
		}

		public void Delete()
		{
			Remove();
			SaveData.Delete();
		}
	}
}