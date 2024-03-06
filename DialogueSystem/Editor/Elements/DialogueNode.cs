using DialogueSystem.Data;
using DialogueSystem.Editor.Extensions;
using DialogueSystem.Editor.Window;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Elements
{
	public sealed class DialogueNode : Node
	{
		public NodeType Type => SaveData.Choices.Count == 0 ? NodeType.Text : NodeType.Prompt;
		
		public readonly NodeSaveData SaveData;

		private readonly DialogueGraphView graphView;

		private readonly Port inputPort, outputPort;

		private readonly ChoicesDisplay choicesDisplay;

		public DialogueNode(Vector2 position, DialogueGraphView graphView, int startingChoices = 0)
		{
			SaveData = NodeSaveData.Create();
			SaveData.Name = "New Node";
			SaveData.Position = position;
			SetPosition(new Rect(position, Vector2.zero));

			this.graphView = graphView;

			this.AddStyleSheet("Nodes/DialogueNode");
			
			RegisterCallback<FocusInEvent>(_ => SaveData.FocusIn());
			RegisterCallback<FocusOutEvent>(_ => SaveData.FocusOut());

			#region Title Container
			inputPort = ElementExtensions.CreatePort(Direction.Input, Port.Capacity.Multi);
			titleButtonContainer.Insert(0, inputPort);
			
			titleButtonContainer.InsertTextField(1, e => SaveData.Name = e.newValue, SaveData.Name);
			titleButtonContainer.InsertIconButton(2, "Add", () => choicesDisplay!.Add());

			outputPort = ElementExtensions.CreatePort(Direction.Output, Port.Capacity.Single);
			titleButtonContainer.Add(outputPort);
			#endregion

			#region Extension Container
			extensionContainer.AddTextField(e => SaveData.Text = e.newValue, SaveData.Text, true);

			choicesDisplay = new ChoicesDisplay(this, graphView);
			extensionContainer.Add(choicesDisplay);

			for (int i = 0; i < startingChoices; i++)
				choicesDisplay.Add();

			expanded = true;
			RefreshExpandedState();
			#endregion
		}

		#region Ports
		public override void BuildContextualMenu(ContextualMenuPopulateEvent e)
		{
			e.menu.AppendAction("Disconnect Input Ports", _ => DisconnectInputPort());
			e.menu.AppendAction("Disconnect Output Ports", _ => DisconnectOutputPorts());
			e.menu.AppendAction("Disconnect All Ports", _ => DisconnectAllPorts());
			e.menu.AppendSeparator();
		}

		public void ShowOutputPort() => outputPort.style.display = DisplayStyle.Flex;
		public void HideOutputPort()
		{
			graphView.DeleteElements(outputPort.connections);
			outputPort.style.display = DisplayStyle.None;
		}

		public void DisconnectAllPorts()
		{
			DisconnectInputPort();
			DisconnectOutputPorts();
		}

		private void DisconnectInputPort() => graphView.DeleteElements(inputPort.connections);
		private void DisconnectOutputPorts()
		{
			graphView.DeleteElements(outputPort.connections);

			foreach (var choiceDisplay in choicesDisplay.Children)
				choiceDisplay.DisconnectPort();
		}
		#endregion
	}
}