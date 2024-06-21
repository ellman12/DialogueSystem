using System.Collections.Generic;
using System.Linq;
using DialogueSystem.Data;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Elements
{
	public sealed class ChoicesDisplay : VisualElement
	{
		public new IEnumerable<ChoiceDisplay> Children => contentContainer.Children().Cast<ChoiceDisplay>();

		public DialogueNode Node { get; private set; }

		public ChoicesDisplay(DialogueNode node)
		{
			Node = node;
		}

		public void Add()
		{
			Node.HideOutputPort();

			ChoiceSaveData choice = new();
			Node.SaveData.Choices.Add(choice);
            Node.SaveData.Save();
			contentContainer.Add(new ChoiceDisplay(this, choice));
		}

		public void Add(ChoiceSaveData saveData)
		{
			Node.HideOutputPort();
			
			contentContainer.Add(new ChoiceDisplay(this, saveData));
		}

		public void Remove(ChoiceDisplay choiceDisplay, ChoiceSaveData saveData)
		{
			Node.SaveData.Choices.Remove(saveData);
			Node.SaveData.Save();
			
			contentContainer.Remove(choiceDisplay);

			if (childCount == 0)
				Node.ShowOutputPort();
		}
	}
}