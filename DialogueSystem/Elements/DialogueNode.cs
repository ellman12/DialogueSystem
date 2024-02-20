using System;
using DialogueSystem.Data;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogueSystem.Elements
{
	public abstract class DialogueNode : Node
	{
		public Guid Id = Guid.NewGuid();
		public string Name;
		public string Text;
		public DialogueType Type;
		public Vector2 Position;
	}
}