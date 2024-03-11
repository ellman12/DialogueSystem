using System.Collections.Generic;
using System.Linq;
using DialogueSystem.Data;
using DialogueSystem.Editor.Window;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Elements
{
	public sealed class ChoicesDisplay : VisualElement
	{
		public new IEnumerable<ChoiceDisplay> Children => contentContainer.Children().Cast<ChoiceDisplay>();

		public readonly DialogueGraphView GraphView;

		private readonly DialogueNode node;

		public ChoicesDisplay(DialogueNode node, DialogueGraphView graphView)
		{
			this.node = node;
			GraphView = graphView;
		}

		public void Add()
		{
			node.HideOutputPort();

			ChoiceSaveData choice = new();
			node.SaveData.Choices.Add(choice);
			contentContainer.Add(new ChoiceDisplay(this, choice));
		}

		public void Add(ChoiceSaveData saveData)
		{
			node.HideOutputPort();
			
			contentContainer.Add(new ChoiceDisplay(this, saveData));
		}

		public void Remove(ChoiceDisplay choiceDisplay, ChoiceSaveData saveData)
		{
			node.SaveData.Choices.Remove(saveData);
			node.SaveData.Save();
			
			contentContainer.Remove(choiceDisplay);

			if (childCount == 0)
				node.ShowOutputPort();
		}
	}
}