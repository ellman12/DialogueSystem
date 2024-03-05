using DialogueSystem.Data;
using DialogueSystem.Editor.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Elements
{
	public sealed class ChoiceDisplay : VisualElement
	{
		private readonly ChoicesDisplay choicesDisplay;
		
		private readonly Port outputPort;
		
		public ChoiceDisplay(ChoicesDisplay choicesDisplay, ChoiceSaveData saveData)
		{
			this.choicesDisplay = choicesDisplay;
			
			AddToClassList("choiceDisplay");
			this.AddStyleSheet("Nodes/ChoiceDisplay");

			Add(ElementUtility.CreateIconButton("Close", () => choicesDisplay.Remove(this, saveData)));
			Add(ElementUtility.CreateTextArea(saveData.Text, "", e => saveData.Text = e.newValue));

			outputPort = ElementUtility.CreatePort(Direction.Output, Port.Capacity.Single);
			Add(outputPort);	
			
			RegisterCallback<DetachFromPanelEvent>(_ => DisconnectPort());
		}

		public void DisconnectPort() => choicesDisplay.GraphView.DeleteElements(outputPort.connections);
	}
}