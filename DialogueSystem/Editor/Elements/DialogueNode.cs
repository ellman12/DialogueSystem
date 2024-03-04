using System;
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
		public Guid Id = Guid.NewGuid();

		public string Name
		{
			get => name;
			set => name = value;
		}
		
		public string Text = "";
		public DialogueType Type;
		public Vector2 Position;
		
		public DialogueGroup Group;
		public DialogueGraphView GraphView;

		public DialogueNode(Vector2 position)
		{
			Name = "New Node";
			
			Position = position;
			SetPosition(new Rect(position, Vector2.zero));
			
			this.AddStyleSheet("Nodes/DialogueNode");
			
			titleButtonContainer.Insert(0, this.CreatePort(Direction.Input, Port.Capacity.Multi));
			titleButtonContainer.Insert(1, ElementUtility.CreateTextField(Name, "", e => Name = e.newValue));
			
			extensionContainer.Add(ElementUtility.CreateTextArea(Text, "", e => Text = e.newValue));
			
			RefreshExpandedState();
		}

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

        private void DisconnectInputPorts() => DisconnectPorts(Direction.Input);

		private void DisconnectOutputPorts() => DisconnectPorts(Direction.Output);

		private void DisconnectPorts(Direction direction)
        {
			foreach (var port in titleButtonContainer.Children().OfType<Port>().Where(child => child.connected && child.direction == direction))
				GraphView.DeleteElements(port.connections);
        }
		#endregion
	}
}