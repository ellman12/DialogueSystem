using DialogueSystem.Editor.Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DialogueSystem.Data;
using DialogueSystem.Editor.Extensions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace DialogueSystem.Editor.Window
{
	public sealed class DialogueGraphView : GraphView
	{
		public string GraphName { get; set; }

		public string GraphPath { get; set; }

		private readonly DialogueGraphWindow window;

		public DialogueGraphView(DialogueGraphWindow window)
		{
			this.window = window;

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

			AddMenuItem("Create Node", e => AddElement(new DialogueNode(this, GetLocalMousePosition(e))));
			AddMenuItem("Create Node With Two Choices", e => AddElement(new DialogueNode(this, GetLocalMousePosition(e), 2)));
			AddMenuItem("Create Group", e => AddElement(new DialogueGroup(GetLocalMousePosition(e))));
			#endregion
		}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter _) => ports.Where(port => startPort != port && startPort.node != port.node && startPort.direction != port.direction).ToList();

		public void LoadGraph(string fullPath)
		{
			SetGraph(fullPath);

			var nodeAssets = DialogueGraphWindow.GetAssetsAtPath<NodeSaveData>(GraphPath);

			Dictionary<string, DialogueNode> dialogueNodes = new(nodeAssets.Length);

			foreach (var asset in nodeAssets)
			{
				DialogueNode node = new(this, asset);
				dialogueNodes.Add(node.SaveData.Id, node);
				AddElement(node);
			}

			foreach (var node in dialogueNodes.Values)
			{
				if (node.SaveData.Next != null)
				{
					var next = dialogueNodes[node.SaveData.Next.Id].Input;
					AddElement(node.Output.ConnectTo(next));
				}
				else
				{
					foreach (var choiceDisplay in node.ChoicesDisplay.Children.Where(display => display.SaveData.Node != null))
					{
						var next = dialogueNodes[choiceDisplay.SaveData.Node.Id].Input;
						AddElement(choiceDisplay.Output.ConnectTo(next));
					}
				}
			}
		}

		public void CreateGraph(string fullPath)
		{
			SetGraph(fullPath);

			Directory.CreateDirectory(fullPath);
			Directory.CreateDirectory(Path.Combine(fullPath, "Ungrouped"));
			Directory.CreateDirectory(Path.Combine(fullPath, "Groups"));
			AssetDatabase.Refresh();
		}

		public void CloseGraph()
		{
			GraphName = GraphPath = "";
			window.titleContent = new GUIContent("Dialogue Graph");
			Clear();
			this.Hide();
		}

		private void SetGraph(string fullPath)
		{
			GraphName = Path.GetFileName(fullPath);
			window.titleContent = new GUIContent($"{GraphName}");
			GraphPath = DialogueGraphWindow.GetRelativePath(fullPath);
			Clear();
			this.Show();
		}

		private new void Clear()
		{
			foreach (var element in graphElements)
				RemoveElement(element); //Using DeleteElements would cause the SOs to also be removed.
		}

		#region Menu
		public override void BuildContextualMenu(ContextualMenuPopulateEvent _) {}

		private void AddMenuItem(string title, Action<DropdownMenuAction> action) => this.AddManipulator(new ContextualMenuManipulator(menuEvent => menuEvent.menu.AppendAction(title, action)));

		private Vector2 GetLocalMousePosition(DropdownMenuAction action) => contentViewContainer.WorldToLocal(action.eventInfo.localMousePosition);
		#endregion

		#region Events
		private static void NodesAddedToGroup(Group group, IEnumerable<GraphElement> elements)
		{
			var dialogueGroup = (DialogueGroup) group;

			foreach (var element in elements.Cast<DialogueNode>())
			{
				element.SaveData.Group = dialogueGroup;
				element.SaveData.Save();
			}
		}

		private static void NodesRemovedFromGroup(Group group, IEnumerable<GraphElement> elements)
		{
			foreach (var element in elements.Cast<DialogueNode>())
			{
				element.SaveData.Group = null;
				element.SaveData.Save();
			}
		}

		#region GraphViewChanged
		private static GraphViewChange UpdateElementPositions(GraphViewChange change)
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

		private static GraphViewChange UpdateElementEdges(GraphViewChange change)
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

		private static GraphViewChange DeleteSelected(GraphViewChange change)
		{
			if (change.elementsToRemove == null)
				return change;

			foreach (var element in change.elementsToRemove.ToArray())
			{
				if (element is DialogueNode node)
					node.Delete();
				else if (element is Edge edge)
                    edge.Delete();
			}

			return change;
		}
		#endregion
		#endregion
	}
}