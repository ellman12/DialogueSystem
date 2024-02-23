using System;
using DialogueSystem.Data;
using DialogueSystem.Editor.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogueSystem.Editor.Elements
{
	public abstract class DialogueNode : Node
	{
		public Guid Id = Guid.NewGuid();

		private new string name = "New Node";
		public string Name
		{
			get => name;
			set
			{
				name = Name;
			}
		}
		
		public string Text;
		public DialogueType Type;
		public Vector2 Position;

		protected DialogueNode(DialogueType type)
		{
			Type = type;
			
			this.AddStyleSheet("Node");
			
			titleButtonContainer.Insert(0, this.CreatePort(Direction.Input, Port.Capacity.Multi));
			titleButtonContainer.Insert(1, ElementUtility.CreateTextField(Name, "", e => Name = e.newValue));
			
			expanded = true;
			RefreshExpandedState();
		}
	}
}