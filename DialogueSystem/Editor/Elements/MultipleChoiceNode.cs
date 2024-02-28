using System.Collections.Generic;
using DialogueSystem.Data;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Editor.Window;
using UnityEngine;

namespace DialogueSystem.Editor.Elements
{
	public sealed class MultipleChoiceNode : DialogueNode
	{
		public readonly List<ChoiceDisplay> ChoiceDisplays = new();
		
		public MultipleChoiceNode(DialogueGraphView graphView, Vector2 position)
		{
			this.AddStyleSheet("Nodes/MultipleChoice");
			AddToClassList("multipleChoiceNode");
			
			GraphView = graphView;
			Type = DialogueType.MultipleChoice;
			Position = position;
            SetPosition(new Rect(position, Vector2.zero));
			
			ChoiceDisplays.Add(new ChoiceDisplay(this));

			titleButtonContainer.Insert(2, ElementUtility.CreateIconButton("Add", AddBtnClick));

			RenderChoices();
		}

		private void RenderChoices()
		{
			foreach (var choiceDisplay in ChoiceDisplays)
				extensionContainer.Add(choiceDisplay);
		}

		private void AddBtnClick()
		{
			ChoiceDisplays.Add(new ChoiceDisplay(this));
			expanded = true;
			RenderChoices();
		}

		public void RemoveChoice(ChoiceDisplay choiceDisplay)
		{
			if (ChoiceDisplays.Count > 1)
			{
				choiceDisplay.Remove();
				RenderChoices();
			}
		}
	}
}