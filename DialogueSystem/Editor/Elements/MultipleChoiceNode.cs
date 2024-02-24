using DialogueSystem.Data;
using UnityEngine;

namespace DialogueSystem.Editor.Elements
{
	public sealed class MultipleChoiceNode : DialogueNode
	{
		public MultipleChoiceNode(Vector2 position)
		{
			Type = DialogueType.MultipleChoice;
			Position = position;
            SetPosition(new Rect(position, Vector2.zero));
		}
	}
}