using System.Linq;
using DialogueSystem.Data;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Editor.Window;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Elements
{
	public sealed class DialogueNode : Node
	{
		public readonly NodeSaveData SaveData;

		public readonly DialogueGraphView GraphView;

		private readonly Port outputPort;

		public readonly VisualElement choiceDisplays = new();

		public DialogueNode(Vector2 position, DialogueGraphView graphView)
		{
			SaveData = NodeSaveData.Create();
			SaveData.Name = "New Node";
			SaveData.Position = position;
			SetPosition(new Rect(position, Vector2.zero));

			GraphView = graphView;

			this.AddStyleSheet("Nodes/DialogueNode");

			titleButtonContainer.Insert(0, this.CreatePort(Direction.Input, Port.Capacity.Multi));
			titleButtonContainer.Insert(1, ElementUtility.CreateTextField(SaveData.Name, "", e => SaveData.Name = e.newValue));
			titleButtonContainer.Insert(2, ElementUtility.CreateIconButton("Add", AddChoice));

			outputPort = this.CreatePort(Direction.Output, Port.Capacity.Single);
			titleButtonContainer.Add(outputPort);

			extensionContainer.Add(ElementUtility.CreateTextArea(SaveData.Text, "", e => SaveData.Text = e.newValue));
			extensionContainer.Add(choiceDisplays);

			expanded = true;
			RefreshExpandedState();
		}

		private void AddChoice()
		{
			SaveData.Next = null;
			GraphView.DeleteElements(outputPort.connections);
			HideOutputPort();

			ChoiceSaveData newChoice = new();
			SaveData.Choices.Add(newChoice);
			choiceDisplays.Add(new ChoiceDisplay(this, newChoice));
		}

		#region Events
		public void OnChoiceRemoved()
		{
			if (choiceDisplays.childCount == 0)
				ShowOutputPort();
		}
		#endregion

		#region Ports
		public override void BuildContextualMenu(ContextualMenuPopulateEvent e)
		{
			e.menu.AppendAction("Disconnect Input Ports", _ => DisconnectInputPorts());
			e.menu.AppendAction("Disconnect Output Ports", _ => DisconnectOutputPorts());
			e.menu.AppendAction("Disconnect All Ports", _ => DisconnectAllPorts());
			e.menu.AppendSeparator();
		}

		public void DisconnectAllPorts()
		{
			DisconnectInputPorts();
			DisconnectOutputPorts();
		}

		private void HideOutputPort() => outputPort.style.display = DisplayStyle.None;
		private void ShowOutputPort() => outputPort.style.display = DisplayStyle.Flex;

		private void DisconnectInputPorts() => DisconnectPorts(Direction.Input);

		private void DisconnectOutputPorts() => DisconnectPorts(Direction.Output);

		private void DisconnectPorts(Direction direction)
		{
			foreach (var port in titleButtonContainer.Children().OfType<Port>().Where(child => child.connected && child.direction == direction))
				GraphView.DeleteElements(port.connections);
			
			foreach (var choiceDisplay in choiceDisplays.Children().Cast<ChoiceDisplay>())
				choiceDisplay.DisconnectPort();
		}
		#endregion
	}
}