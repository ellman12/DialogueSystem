using DialogueSystem.Data;
using DialogueSystem.Editor.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogueSystem.Editor.Elements
{
	public sealed class SingleChoiceNode : DialogueNode
	{
		public SingleChoiceNode(Vector2 position)
		{
			titleButtonContainer.Add(this.CreatePort(Direction.Output, Port.Capacity.Single));
			
			Type = DialogueType.SingleChoice;
			Position = position;
            SetPosition(new Rect(position, Vector2.zero));
		}
	}
}