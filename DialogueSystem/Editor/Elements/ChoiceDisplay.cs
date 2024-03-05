using DialogueSystem.Data;
using DialogueSystem.Editor.Extensions;
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
			
			this.AddIconButton("Close", () => choicesDisplay.Remove(this, saveData));
			this.AddTextField(e => saveData.Text = e.newValue, saveData.Text);

			outputPort = ElementExtensions.CreatePort(Direction.Output, Port.Capacity.Single);
			Add(outputPort);	
			
			RegisterCallback<DetachFromPanelEvent>(_ => DisconnectPort());
		}

		public void DisconnectPort() => choicesDisplay.GraphView.DeleteElements(outputPort.connections);
	}
}