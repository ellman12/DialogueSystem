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
			deleteSelection = (_, _) => OnElementsDeleted();

			elementsAddedToGroup = NodesAddedToGroup;
			elementsRemovedFromGroup = NodesRemovedFromGroup;
			#endregion

			#region Manipulators
			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());

			AddMenuItem("Create Node", e => AddElement(new DialogueNode(GetLocalMousePosition(e), this)));
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
			var dialogueGroup = (DialogueGroup) group;

			foreach (var element in elements.Cast<DialogueNode>())
				element.SaveData.GroupId = dialogueGroup.Id;
		}
		
		private void NodesRemovedFromGroup(Group group, IEnumerable<GraphElement> elements)
		{
			foreach (var element in elements.Cast<DialogueNode>())
				element.SaveData.GroupId = "";
		}
		#endregion
	}
}