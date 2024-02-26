using DialogueSystem.Data;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Editor.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace DialogueSystem.Editor.Window
{
	public sealed class DialogueGraphView : GraphView
	{
		public DialogueGraphView(DialogueGraphWindow window)
		{
			this.StretchToParentSize();
			window.rootVisualElement.Add(this);

			GridBackground gridBackground = new();
			gridBackground.StretchToParentSize();
			Insert(0, gridBackground);

			this.AddStyleSheet("GraphView");

			#region Events
			deleteSelection = (_, _) => OnElementsDeleted();

			elementsAddedToGroup = NodesAddedToGroup;
			elementsRemovedFromGroup = NodesRemovedFromGroup;
			#endregion

			#region Manipulators
			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());

			this.AddManipulator(CreateNodeContextualMenu("Add Single Choice Node", DialogueType.SingleChoice));
			this.AddManipulator(CreateNodeContextualMenu("Add Multiple Choice Node", DialogueType.MultipleChoice));
			this.AddManipulator(CreateGroupContextualMenu("Add Group"));
			#endregion
		}

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) {}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter _)
		{
			return ports.Where(port => startPort != port && startPort.node != port.node && startPort.direction != port.direction).ToList();
        }

		#region Menu
		private IManipulator CreateNodeContextualMenu(string title, DialogueType type) => new ContextualMenuManipulator(menuEvent => menuEvent.menu.AppendAction(title, e => CreateNode(type, GetLocalMousePosition(e))));

		private void CreateNode(DialogueType type, Vector2 position)
		{
			DialogueNode node = type switch
			{
				DialogueType.SingleChoice => new SingleChoiceNode(this, position),
				DialogueType.MultipleChoice => new MultipleChoiceNode(this, position),
				_ => throw new ArgumentOutOfRangeException()
			};
			
			AddElement(node);
		}
		
		private IManipulator CreateGroupContextualMenu(string title) => new ContextualMenuManipulator(menuEvent => menuEvent.menu.AppendAction(title, e => CreateGroup(GetLocalMousePosition(e))));

		private void CreateGroup(Vector2 position)
		{
			AddElement(new DialogueGroup(position));
		}
		
        private Vector2 GetLocalMousePosition(DropdownMenuAction action) => contentViewContainer.WorldToLocal(action.eventInfo.localMousePosition);
		#endregion

		#region Events
		private void OnElementsDeleted()
		{
			foreach (var element in selection.Cast<GraphElement>().ToArray())
			{
				if (element is DialogueNode node)
					node.DisconnectAllPorts();
				
				element.RemoveFromHierarchy();
			}
		}

		private void NodesAddedToGroup(Group group, IEnumerable<GraphElement> elements)
		{
			var dialogueGroup = group as DialogueGroup;

			foreach (var element in elements.Cast<DialogueNode>())
				element.Group = dialogueGroup;
		}
		
		private void NodesRemovedFromGroup(Group group, IEnumerable<GraphElement> elements)
		{
			foreach (var element in elements.Cast<DialogueNode>())
				element.Group = null;
		}
		#endregion
	}
}