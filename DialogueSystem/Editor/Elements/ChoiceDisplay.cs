using DialogueSystem.Data;
using DialogueSystem.Editor.Extensions;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Elements
{
	public sealed class ChoiceDisplay : VisualElement
	{
		public ChoiceSaveData SaveData { get; private set; }
		
		public Port Output { get; private set; }
		
		private readonly ChoicesDisplay choicesDisplay;
		
		public ChoiceDisplay(ChoicesDisplay choicesDisplay, ChoiceSaveData saveData)
		{
			this.choicesDisplay = choicesDisplay;
			SaveData = saveData;
			
			AddToClassList("choiceDisplay");
			this.AddStyleSheet("Nodes/ChoiceDisplay");
			
			this.AddIconButton("Close", () => { DisconnectPort(); choicesDisplay.Remove(this, saveData); });
			this.AddTextField(e => saveData.Text = e.newValue, saveData.Text);

			Output = ElementExtensions.CreatePort(Direction.Output, Port.Capacity.Single);
			Output.userData = saveData;
			Add(Output);	
		}

		public void DisconnectPort() => choicesDisplay.GraphView.DeleteElements(Output.connections);
	}
}