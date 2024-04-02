using System;
using System.Linq;
using DialogueSystem.Data;
using DialogueSystem.Editor.Elements.Interfaces;
using DialogueSystem.Editor.Window;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Elements
{
	[Serializable]
	public sealed class DialogueGroup : Group, ISaveableElement<GroupSaveData>
	{
		public GroupSaveData SaveData { get; }

		#region Constructors
		public DialogueGroup(Vector2 position)
		{
			SaveData = GroupSaveData.Create(position);
			
			RegisterCallback<FocusOutEvent>(_ => FocusOut());
			
			title = SaveData.Id;
			SetPosition(new Rect(position, Vector2.zero));
		}
		
		public DialogueGroup(GroupSaveData saveData)
		{
			SaveData = saveData;
			
			RegisterCallback<FocusOutEvent>(_ => FocusOut());

			title = SaveData.Name;
			SetPosition(new Rect(SaveData.Position, Vector2.zero));
		}
	
		private void FocusOut()
		{
			SaveData.Name = title = title.Trim();
			SaveData.Save();
		}	
		#endregion

		public void UpdatePosition(Vector2 newPosition)
		{
			SaveData.Position = newPosition;
			SaveData.Save();
			
			foreach (var node in containedElements.Cast<DialogueNode>())
				node.UpdatePosition(node.GetPosition().position);
		}

		public void Remove() => DialogueGraphView.C.RemoveElement(this);

		public void Delete()
		{
			Remove();
			SaveData.Delete();
		}
	}
}