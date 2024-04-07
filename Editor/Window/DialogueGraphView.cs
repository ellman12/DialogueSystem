using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DialogueSystem.Data;
using DialogueSystem.Editor.Elements;
using DialogueSystem.Editor.Elements.Interfaces;
using DialogueSystem.Editor.Extensions;
using DialogueSystem.Editor.Utilities;
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

        public static DialogueGraphView C => DialogueGraphWindow.GraphView;

        public DialogueGraphView()
        {
            this.StretchToParentSize();
            this.AddStyleSheet("GraphView");

            GridBackground gridBackground = new();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);

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

            AddMenuItem("Create Node", e => AddElement(new DialogueNode(GetLocalMousePosition(e))));
            AddMenuItem("Create Node With Two Choices", e => AddElement(new DialogueNode(GetLocalMousePosition(e), 2)));
            AddMenuItem("Create Group", e => AddElement(new DialogueGroup(GetLocalMousePosition(e))));
            #endregion
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter _) => ports.Where(port => startPort != port && startPort.node != port.node && startPort.direction != port.direction).ToList();

        public void LoadGraph(string path)
        {
            path = PathUtility.GetRelativePath(path);
            
            if (DialogueGraphWindow.Windows.Any(window => window != DialogueGraphWindow.C && window.graphView.GraphPath == path))
            {
                DialogueGraphToolbar.C.Error.text = "Graph open in another window";
                return;
            }
            
            SetGraph(path);

            var nodeAssets = IOUtility.GetAssetsAtPath<NodeSaveData>(GraphPath);
            var groupAssets = IOUtility.GetAssetsAtPath<GroupSaveData>(GraphPath);

            Dictionary<string, DialogueNode> dialogueNodes = new(nodeAssets.Length);
            Dictionary<string, DialogueGroup> groups = new(groupAssets.Length);

            //TODO: clean up this war crime at some point.
            foreach (var asset in groupAssets)
            {
                DialogueGroup group = new(asset);
                groups.Add(group.SaveData.Id, group);
                AddElement(group);
            }

            foreach (var asset in nodeAssets)
            {
                DialogueNode node = new(asset);
                dialogueNodes.Add(node.SaveData.Id, node);
                AddElement(node);
            }

            foreach (var node in dialogueNodes.Values)
            {
                if (node.SaveData.Next != null)
                {
                    var next = dialogueNodes[node.SaveData.Next.Id].Input;
                    AddElement(node.Output.ConnectTo<DialogueEdge>(next));
                }
                else
                {
                    foreach (var choiceDisplay in node.ChoicesDisplay.Children.Where(display => display.SaveData.Node != null))
                    {
                        var next = dialogueNodes[choiceDisplay.SaveData.Node.Id].Input;
                        AddElement(choiceDisplay.Output.ConnectTo<DialogueEdge>(next));
                    }
                }
            }

            foreach (DialogueNode node in dialogueNodes.Values.Where(node => node.SaveData.Group != null))
            {
                groups[node.SaveData.Group.Id].AddElement(node);
            }
        }

		public void CreateGraph(string path)
		{
			path = PathUtility.GetRelativePath(path);
			string ungroupedPath = PathUtility.Combine(path, "Ungrouped");
			string groupedPath = PathUtility.Combine(path, "Groups");

            if (Directory.Exists(ungroupedPath) && Directory.Exists(groupedPath))
            {
                LoadGraph(path);
                return;
            }

            SetGraph(path);
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(ungroupedPath);
            Directory.CreateDirectory(groupedPath);
            AssetDatabase.Refresh();
        }

		public void CloseGraph()
		{
			GraphName = GraphPath = "";
			DialogueGraphWindow.C.SetTitle("Dialogue Graph");
			DialogueGraphToolbar.C.Error.text = "";
			Clear();
			this.Hide();
		}
        
        public void OnTextInputFocusIn(DialogueNode node)
        {
            ClearSelection();
            AddToSelection(node);
        }

        private void SetGraph(string path)
        {
            GraphName = Path.GetFileName(path);
            DialogueGraphWindow.C.SetTitle(GraphName);
            GraphPath = PathUtility.GetRelativePath(path);
            Clear();
            this.Show();
        }

        private new void Clear()
        {
            foreach (var element in graphElements.OfType<IDialogueElement>())
                element.Remove();
        }

        #region Menu
        public override void BuildContextualMenu(ContextualMenuPopulateEvent _) {}

        private void AddMenuItem(string title, Action<DropdownMenuAction> action) => this.AddManipulator(new ContextualMenuManipulator(menuEvent => menuEvent.menu.AppendAction(title, action)));

        private Vector2 GetLocalMousePosition(DropdownMenuAction action) => contentViewContainer.WorldToLocal(action.eventInfo.localMousePosition);
        #endregion

		#region Events
		private static void NodesAddedToGroup(Group group, IEnumerable<GraphElement> nodes)
		{
			var dialogueGroup = (DialogueGroup) group;

			foreach (var node in nodes.Cast<DialogueNode>())
			{
                node.SaveData.Group = dialogueGroup.SaveData;
                node.SaveData.Save();
            }
        }

        private static void NodesRemovedFromGroup(Group group, IEnumerable<GraphElement> nodes)
        {
            foreach (var node in nodes.Cast<DialogueNode>())
            {
                node.SaveData.Group = null;
                node.SaveData.Save();
            }
        }

        #region GraphViewChanged
        private static GraphViewChange UpdateElementPositions(GraphViewChange change)
        {
            if (change.movedElements == null)
                return change;

            foreach (var element in change.movedElements)
            {
                if (element is ISaveableElement<SaveData> saveableElement)
                    saveableElement.UpdatePosition(element.GetPosition().position);
            }

            return change;
        }

        private static GraphViewChange UpdateElementEdges(GraphViewChange change)
        {
            if (change.edgesToCreate == null)
                return change;

			foreach (var edge in change.edgesToCreate.Cast<DialogueEdge>())
				edge.OnConnect();

            return change;
        }

        private static GraphViewChange DeleteSelected(GraphViewChange change)
        {
            if (change.elementsToRemove == null)
                return change;

            foreach (var element in change.elementsToRemove.ToArray().OfType<IDialogueElement>())
                element.Delete();

            return change;
        }
        #endregion
        #endregion
    }
}