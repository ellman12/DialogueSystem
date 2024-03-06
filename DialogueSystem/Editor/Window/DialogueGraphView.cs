using DialogueSystem.Editor.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem.Data;
using DialogueSystem.Editor.Extensions;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace DialogueSystem.Editor.Window
{
	public sealed class DialogueGraphView : GraphView
	{
		public const string GraphsRootPath = "Assets/DialogueSystem/Graphs";

		public DialogueGraphView(DialogueGraphWindow window)
		{
			this.StretchToParentSize();
			window.rootVisualElement.Add(this);

			GridBackground gridBackground = new();
			gridBackground.StretchToParentSize();
			Insert(0, gridBackground);

			this.AddStyleSheet("GraphView");

			#region Events
			elementsAddedToGroup = NodesAddedToGroup;
			elementsRemovedFromGroup = NodesRemovedFromGroup;

			graphViewChanged += UpdateElementPositions;
			graphViewChanged += UpdateElementEdges;
			graphViewChanged += DeleteSelected;
			#endregion

			#region Manipulators
			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());

			AddMenuItem("Create Node", e => AddElement(new DialogueNode(GetLocalMousePosition(e), this)));
			AddMenuItem("Create Node With Two Choices", e => AddElement(new DialogueNode(GetLocalMousePosition(e), this, 2)));
			AddMenuItem("Create Group", e => AddElement(new DialogueGroup(GetLocalMousePosition(e))));
			#endregion
		}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter _) => ports.Where(port => startPort != port && startPort.node != port.node && startPort.direction != port.direction).ToList();

		#region Menu
		public override void BuildContextualMenu(ContextualMenuPopulateEvent _) {}

		private void AddMenuItem(string title, Action<DropdownMenuAction> action) => this.AddManipulator(new ContextualMenuManipulator(menuEvent => menuEvent.menu.AppendAction(title, action)));

		private Vector2 GetLocalMousePosition(DropdownMenuAction action) => contentViewContainer.WorldToLocal(action.eventInfo.localMousePosition);
		#endregion

		#region Events
		private void NodesAddedToGroup(Group group, IEnumerable<GraphElement> elements)
		{
			var dialogueGroup = (DialogueGroup) group;

			foreach (var element in elements.Cast<DialogueNode>())
				element.SaveData.GroupId = dialogueGroup.Id;
		}

		private void NodesRemovedFromGroup(Group group, IEnumerable<GraphElement> elements)
		{
			foreach (var element in elements.Cast<DialogueNode>())
				element.SaveData.GroupId = "";
		}

		#region GraphViewChanged
		private GraphViewChange UpdateElementPositions(GraphViewChange change)
		{
			if (change.movedElements == null)
				return change;

			foreach (var element in change.movedElements)
			{
				if (element is DialogueNode node)
					node.SaveData.Position = element.GetPosition().position;
				else if (element is DialogueGroup group)
					group.Position = element.GetPosition().position;
			}

			return change;
		}

		private GraphViewChange UpdateElementEdges(GraphViewChange change)
		{
			if (change.edgesToCreate == null)
				return change;

			foreach (var edge in change.edgesToCreate)
			{
				DialogueNode startNode = edge.GetStartNode();
				DialogueNode endNode = edge.GetEndNode();

				if (startNode.Type == NodeType.Text)
					startNode.SaveData.Next = endNode.SaveData;
				else
				{
					var saveData = (ChoiceSaveData) edge.output.userData;
					saveData.Node = endNode.SaveData;
				}
				
				startNode.SaveData.Save();
			}

			return change;
		}

		private GraphViewChange DeleteSelected(GraphViewChange change)
		{
			if (change.elementsToRemove == null)
				return change;

			foreach (var element in change.elementsToRemove.ToArray())
			{
				if (element is DialogueNode node)
					node.DisconnectAllPorts();
				else if (element is Edge edge)
				{
					edge.input.Disconnect(edge);
					edge.output.Disconnect(edge);

					if (edge.output.userData != null)
					{
						var saveData = (ChoiceSaveData) edge.output.userData;
						saveData.Node = null;
						edge.GetStartNode().SaveData.Save();
					}
				}

				element.RemoveFromHierarchy();
			}

			return change;
		}
		#endregion
		
		#endregion
	}
}