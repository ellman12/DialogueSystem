using DialogueSystem.Data;
using DialogueSystem.Editor.Extensions;
using DialogueSystem.Editor.Window;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Elements
{
	public sealed class ChoiceDisplay : VisualElement
	{
		public ChoiceSaveData SaveData { get; set; }
		
		public Port Output { get; }
		
		public ChoiceDisplay(ChoicesDisplay choicesDisplay, ChoiceSaveData saveData)
		{
			SaveData = saveData;
			
			AddToClassList("choiceDisplay");
			this.AddStyleSheet("Nodes/ChoiceDisplay");
			
			this.AddIconButton("Close", () => { DisconnectPort(); choicesDisplay.Remove(this, saveData); });
			this.AddTextField(OnTextChange, SaveData.Text);

			Output = ElementExtensions.CreatePort(Direction.Output, Port.Capacity.Single);
			Output.userData = saveData;
			Add(Output);	
		}

		public void DisconnectPort() => DialogueGraphView.C.DeleteElements(Output.connections);
		
		private void OnTextChange(ChangeEvent<string> e) => SaveData.Text = e.newValue;
	}
}