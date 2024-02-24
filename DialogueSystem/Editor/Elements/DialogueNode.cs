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
	public abstract class DialogueNode : Node
	{
		public Guid Id = Guid.NewGuid();

		private new string name = "New Node";
		public string Name
		{
			get => name;
			set
			{
				name = Name;
			}
		}
		
		public string Text = "";
		public DialogueType Type;
		public Vector2 Position;
		protected DialogueGraphView GraphView;

		protected DialogueNode()
		{
			this.AddStyleSheet("Node");
			
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