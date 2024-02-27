using DialogueSystem.Data;
using DialogueSystem.Editor.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Elements
{
	public sealed class ChoiceDisplay : VisualElement
	{
		private readonly ChoiceSaveData SaveData = new();

		public ChoiceDisplay(MultipleChoiceNode parent)
		{
			AddToClassList("choiceDisplay");
			this.AddStyleSheet("Nodes/ChoiceDisplay");

			Add(ElementUtility.CreateIconButton("Close", () => parent.RemoveChoice(this)));
			Add(ElementUtility.CreateTextArea(SaveData.Text, "", e => Debug.Log(e.newValue)));
			Add(parent.CreatePort(Direction.Output, Port.Capacity.Single));
		}
	}
}