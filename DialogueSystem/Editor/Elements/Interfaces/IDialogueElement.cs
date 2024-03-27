namespace DialogueSystem.Editor.Elements.Interfaces
{
	///Shared behavior between any element on the DialogueGraphView: nodes, edges, and groups.
	public interface IDialogueElement
	{
		///Removes the element from the DialogueGraphView, preserving its SaveData asset.
		public void Remove();
	}
}