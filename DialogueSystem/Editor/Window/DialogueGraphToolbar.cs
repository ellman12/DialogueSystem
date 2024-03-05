using DialogueSystem.Editor.Extensions;
using DialogueSystem.Editor.Utilities;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Window
{
	public sealed class DialogueGraphToolbar : Toolbar
	{
		private readonly Button saveButton;
		private readonly TextField fileNameTextField;

		public DialogueGraphToolbar()
		{
			fileNameTextField = ElementExtensions.CreateTextField(e => fileNameTextField!.value = e.newValue, "Dialogue Graph");
			Add(fileNameTextField);

			saveButton = ElementExtensions.CreateButton("Save", Save);
			Add(saveButton);
			
			this.AddButton("Load", Load);

			this.AddStyleSheet("Toolbar");
		}

		private void Save()
		{
			
		}

		private void Load()
		{
			
		}
	}
}