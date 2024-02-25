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

			this.AddStyleSheet("Constants");
			this.AddStyleSheet("GraphView");

			deleteSelection = (_, _) => OnElementsDeleted();
			
			#region Manipulators
			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());

			this.AddManipulator(CreateNodeContextualMenu("Add Single Choice Node", DialogueType.SingleChoice));
			this.AddManipulator(CreateNodeContextualMenu("Add Multiple Choice Node", DialogueType.MultipleChoice));
			#endregion
		}

		private Node CreateNode(DialogueType type, Vector2 position)
		{
			return type switch
			{
				DialogueType.SingleChoice => new SingleChoiceNode(this, position),
				DialogueType.MultipleChoice => new MultipleChoiceNode(this, position),
				_ => throw new ArgumentOutOfRangeException()
			};
		}
		
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter _)
		{
			return ports.Where(port => startPort != port && startPort.node != port.node && startPort.direction != port.direction).ToList();
        }
		
		private IManipulator CreateNodeContextualMenu(string actionTitle, DialogueType type) => new ContextualMenuManipulator(menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(type, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))));

        private Vector2 GetLocalMousePosition(Vector2 mousePosition) => contentViewContainer.WorldToLocal(mousePosition);

		private void OnElementsDeleted()
		{
			foreach (var element in selection.Cast<GraphElement>().ToArray())
			{
				if (element is DialogueNode node)
					node.DisconnectAllPorts();
				
				element.RemoveFromHierarchy();
			}
		}
	}
}