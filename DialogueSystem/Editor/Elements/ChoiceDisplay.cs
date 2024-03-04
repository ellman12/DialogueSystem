using DialogueSystem.Data;
using DialogueSystem.Editor.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Elements
{
	public sealed class ChoiceDisplay : VisualElement
	{
		private new readonly DialogueNode parent;

		private readonly ChoiceSaveData SaveData = new();

		private readonly Port port;

		public ChoiceDisplay(DialogueNode parent)
		{
			AddToClassList("choiceDisplay");
			this.AddStyleSheet("Nodes/ChoiceDisplay");

			Add(ElementUtility.CreateIconButton("Close", Remove));
			Add(ElementUtility.CreateTextArea(SaveData.Text, "", e => Debug.Log(e.newValue)));

			port = parent.CreatePort(Direction.Output, Port.Capacity.Single);
			Add(port);

			this.parent = parent;
		}

		private void Remove()
		{
			// if (parent.ChoiceDisplays.Count > 1)
			// {
				// parent.ChoiceDisplays.Remove(this);
				// parent.extensionContainer.Remove(this);
				// parent.RenderChoices();
			// }
			
			// parent.GraphView.DeleteElements(port.connections);
		}
	}
}