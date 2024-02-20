namespace DialogueSystem.Elements
{
	using UnityEngine;

	public sealed class SingleChoiceNode : DialogueNode
	{
		public SingleChoiceNode(Vector2 position)
		{
			Position = position;
            SetPosition(new Rect(position, Vector2.zero));
		}
	}
}