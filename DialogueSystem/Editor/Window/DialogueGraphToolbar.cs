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
			fileNameTextField = ElementUtility.CreateTextField("Dialogue Graph", "", e => fileNameTextField.value = e.newValue);

			saveButton = ElementUtility.CreateButton("Save", Save);
			Button loadButton = ElementUtility.CreateButton("Load", Load);

			Add(fileNameTextField);
			Add(saveButton);
			Add(loadButton);

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