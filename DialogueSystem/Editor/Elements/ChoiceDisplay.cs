using DialogueSystem.Data;
using DialogueSystem.Editor.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Elements
{
	public sealed class ChoiceDisplay : VisualElement
	{
		private readonly ChoiceSaveData saveData;

		private new readonly DialogueNode parent;

		private readonly Port outputPort;

		public ChoiceDisplay(DialogueNode parent, ChoiceSaveData saveData)
		{
			this.parent = parent;
			this.saveData = saveData;

			AddToClassList("choiceDisplay");
			this.AddStyleSheet("Nodes/ChoiceDisplay");

			Add(ElementUtility.CreateIconButton("Close", Remove));
			Add(ElementUtility.CreateTextArea(saveData.Text, "", e => saveData.Text = e.newValue));

			outputPort = parent.CreatePort(Direction.Output, Port.Capacity.Single);
			Add(outputPort);
		}

		public void DisconnectPort()
		{
			parent.GraphView.DeleteElements(outputPort.connections);
			saveData.NodeId = "";
		}

		private void Remove()
		{
			DisconnectPort();
			parent.SaveData.Choices.Remove(saveData);
			parent.choiceDisplays.Remove(this);
			parent.OnChoiceRemoved();
		}
	}
}