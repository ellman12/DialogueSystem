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

		public Port Input { get; private set; }
		public Port Output { get; private set; }

		public ChoicesDisplay ChoicesDisplay { get; private set; }

		private readonly DialogueGraphView graphView;

		public DialogueNode(DialogueGraphView graphView, Vector2 position, int startingChoices = 0)
		{
			this.graphView = graphView;
			SaveData = NodeSaveData.Create(graphView.GraphPath, position);
			
			AddEvents();
			AddElements();

			for (int i = 0; i < startingChoices; i++)
				ChoicesDisplay.Add();
		}

		public DialogueNode(DialogueGraphView graphView, NodeSaveData saveData)
		{
			this.graphView = graphView;
			SaveData = saveData;

			AddEvents();
			AddElements();

			foreach (var choice in SaveData.Choices)
			{
				ChoicesDisplay.Add(choice);
			}
		}

		private void AddEvents()
		{
			RegisterCallback<FocusInEvent>(_ => SaveData.FocusIn());
			RegisterCallback<FocusOutEvent>(_ => SaveData.FocusOut());
		}

		private void AddElements()
		{
			SetPosition(new Rect(SaveData.Position, Vector2.zero));
			this.AddStyleSheet("Nodes/DialogueNode");

			ChoicesDisplay = new ChoicesDisplay(this, graphView);
			
			Input = ElementExtensions.CreatePort(Direction.Input, Port.Capacity.Multi);
			titleButtonContainer.Insert(0, Input);

			titleButtonContainer.InsertTextField(1, e => SaveData.Name = e.newValue, SaveData.Name);
			titleButtonContainer.InsertIconButton(2, "Add", ChoicesDisplay.Add);

			Output = ElementExtensions.CreatePort(Direction.Output, Port.Capacity.Single);
			titleButtonContainer.Add(Output);

			extensionContainer.AddTextField(e => SaveData.Text = e.newValue, SaveData.Text, true);

			extensionContainer.Add(ChoicesDisplay);

			expanded = true;
			RefreshExpandedState();
		}

        public void Delete()
        {
            DisconnectAllPorts();
            SaveData.Delete();
            RemoveFromHierarchy();
        }

        #region Ports
		public override void BuildContextualMenu(ContextualMenuPopulateEvent e)
		{
			e.menu.AppendAction("Disconnect Input Ports", _ => DisconnectInputPort());
			e.menu.AppendAction("Disconnect Output Ports", _ => DisconnectOutputPorts());
			e.menu.AppendAction("Disconnect All Ports", _ => DisconnectAllPorts());
			e.menu.AppendSeparator();
		}

		public void ShowOutputPort() => Output.style.display = DisplayStyle.Flex;
		public void HideOutputPort()
		{
			SaveData.Next = null;
			SaveData.Save();

			graphView.DeleteElements(Output.connections);
			Output.style.display = DisplayStyle.None;
		}

		private void DisconnectAllPorts()
		{
			DisconnectInputPort();
			DisconnectOutputPorts();
		}

		private void DisconnectInputPort() => graphView.DeleteElements(Input.connections);
		private void DisconnectOutputPorts()
		{
			SaveData.Next = null;
			SaveData.Save();

			graphView.DeleteElements(Output.connections);

			foreach (var choiceDisplay in ChoicesDisplay.Children)
				choiceDisplay.DisconnectPort();
		}
		#endregion
	}
}