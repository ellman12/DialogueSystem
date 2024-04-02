using UnityEngine;

namespace DialogueSystem.Editor.Elements.Interfaces
{
	///Represents a Dialogue Element (like nodes and groups) that can be clicked and dragged around.
	public interface IMoveableElement : IDialogueElement
	{
		public void UpdatePosition(Vector2 newPosition);
	}
}