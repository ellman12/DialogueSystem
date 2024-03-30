using DialogueSystem.Data;

namespace DialogueSystem.Editor.Elements.Interfaces
{
	///Represents a moveable element that has some type of SaveData attached to it.
	public interface ISaveableElement<out TAsset> : IMoveableElement where TAsset : SaveData
	{
		public TAsset SaveData { get; }
	}
}