using UnityEngine;

namespace DialogueSystem.Editor.Elements
{
	public sealed class SingleChoiceNode : DialogueNode
	{
		public SingleChoiceNode(Vector2 position)
		{
			Position = position;
            SetPosition(new Rect(position, Vector2.zero));
		}
	}
}