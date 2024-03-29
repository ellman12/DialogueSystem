using UnityEngine;

namespace DialogueSystem.Editor.Elements.Interfaces
{
	///Represents a Dialogue Element that has some type of SaveData attached to it.
	public interface ISaveableElement<TAsset> : IDialogueElement where TAsset : ScriptableObject
	{
		public TAsset SaveData { get; set; }
	}
}