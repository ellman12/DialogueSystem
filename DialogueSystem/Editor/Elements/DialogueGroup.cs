using System;
using DialogueSystem.Data;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Elements
{
	[Serializable]
	public sealed class DialogueGroup : Group
	{
		public readonly GroupSaveData SaveData;

		public DialogueGroup(Vector2 position)
		{
			SaveData = GroupSaveData.Create(position);

			RegisterCallback<FocusOutEvent>(_ => FocusOut());

			title = SaveData.Id;
			SetPosition(new Rect(position, Vector2.zero));
		}

		private void FocusOut()
		{
			SaveData.Name = title = title.Trim();
			SaveData.Save();
		}
	}
}