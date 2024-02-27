using System.Collections.Generic;
using DialogueSystem.Data;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Editor.Window;
using UnityEngine;

namespace DialogueSystem.Editor.Elements
{
	public sealed class MultipleChoiceNode : DialogueNode
	{
		public readonly List<ChoiceSaveData> Choices = new();
		
		public MultipleChoiceNode(DialogueGraphView graphView, Vector2 position)
		{
			this.AddStyleSheet("Nodes/MultipleChoice");
			AddToClassList("multipleChoiceNode");
			
			GraphView = graphView;
			Type = DialogueType.MultipleChoice;
			Position = position;
            SetPosition(new Rect(position, Vector2.zero));

			titleButtonContainer.Insert(2, ElementUtility.CreateIconButton("Add", AddBtnClick));
		}

		private void AddBtnClick()
		{
			Choices.Add(new ChoiceSaveData());
		}
	}
}