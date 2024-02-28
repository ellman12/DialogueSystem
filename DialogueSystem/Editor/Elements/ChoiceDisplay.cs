using DialogueSystem.Data;
using DialogueSystem.Editor.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Elements
{
	public sealed class ChoiceDisplay : VisualElement
	{
		private new readonly MultipleChoiceNode parent;

		private readonly ChoiceSaveData SaveData = new();

		private readonly Port port;
		
		public ChoiceDisplay(MultipleChoiceNode parent)
		{
			AddToClassList("choiceDisplay");
			this.AddStyleSheet("Nodes/ChoiceDisplay");

			Add(ElementUtility.CreateIconButton("Close", () => parent.RemoveChoice(this)));
			Add(ElementUtility.CreateTextArea(SaveData.Text, "", e => Debug.Log(e.newValue)));
			
			port = parent.CreatePort(Direction.Output, Port.Capacity.Single);
			Add(port);

			this.parent = parent;
		}

		public void Remove()
		{ 
			parent.ChoiceDisplays.Remove(this);
			parent.extensionContainer.Remove(this);
			parent.GraphView.DeleteElements(port.connections);
		}
	}
}