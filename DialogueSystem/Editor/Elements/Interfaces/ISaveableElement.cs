using DialogueSystem.Data;

namespace DialogueSystem.Editor.Elements.Interfaces
{
	///Represents a Dialogue Element that has some type of SaveData attached to it.
	public interface ISaveableElement<out TAsset> : IDialogueElement where TAsset : SaveData
	{
		public TAsset SaveData { get; }
	}
}