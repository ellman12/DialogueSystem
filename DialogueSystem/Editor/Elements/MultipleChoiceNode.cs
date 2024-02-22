using UnityEngine;

namespace DialogueSystem.Editor.Elements
{
	public sealed class MultipleChoiceNode : DialogueNode
	{
		public MultipleChoiceNode(Vector2 position)
		{
			Position = position;
            SetPosition(new Rect(position, Vector2.zero));
		}
	}
}