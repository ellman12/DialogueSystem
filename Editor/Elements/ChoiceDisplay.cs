using DialogueSystem.Data;
using DialogueSystem.Editor.Extensions;
using DialogueSystem.Editor.Window;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Elements
{
	public sealed class ChoiceDisplay : VisualElement
    {
        private ChoiceSaveData saveData;
        public ChoiceSaveData SaveData
        {
            get => saveData;
            set => Output.userData = saveData = value;
        }
		
		public Port Output { get; }
		
		public ChoiceDisplay(ChoicesDisplay choicesDisplay, ChoiceSaveData saveData)
		{
			AddToClassList("choiceDisplay");
			this.AddStyleSheet("Nodes/ChoiceDisplay");
			
			this.AddIconButton("Close", () => { DisconnectPort(); choicesDisplay.Remove(this, saveData); });
			this.AddTextField(OnTextChange, saveData.Text);

			Output = ElementExtensions.CreatePort(Direction.Output, Port.Capacity.Single);
			Output.userData = saveData;
			Add(Output);	
            
			SaveData = saveData;
		}

		public void DisconnectPort() => DialogueGraphView.C.DeleteElements(Output.connections);
		
		private void OnTextChange(ChangeEvent<string> e) => SaveData.Text = e.newValue;
	}
}