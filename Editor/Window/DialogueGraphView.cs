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
        public string GraphName { get; set; } = "";

        public string GraphPath { get; set; } = "";

        public bool GraphOpen => !String.IsNullOrWhiteSpace(GraphPath);

        public IEnumerable<DialogueNode> StartNodes => graphElements.OfType<DialogueNode>().Where(node => !node.Input.connected);
        
        public IEnumerable<DialogueNode> EndNodes => graphElements.OfType<DialogueNode>().Where(node => !node.Output.connected);

        public IEnumerable<DialogueNode> ConnectionlessNodes => graphElements.OfType<DialogueNode>().Where(node => !node.Input.connected && !node.Output.connected);

        public static Vector2 MousePosition;

        public static DialogueGraphView C => DialogueGraphWindow.GraphView;

        private DialogueNode newestNode;

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

            RegisterCallback<MouseMoveEvent>(e => MousePosition = contentViewContainer.WorldToLocal(e.mousePosition));
            #endregion

            #region Manipulators
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            AddMenuItem("Create Node", _ => AddNode());
            AddMenuItem("Create Node With Two Choices", _ => AddNode(2));
            AddMenuItem("Create Group", _ => AddGroup());
            #endregion
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter _) => ports.Where(port => startPort != port && startPort.node != port.node && startPort.direction != port.direction).ToList();

        public async void TryLoadGraph()
        {
            string fullPath = EditorUtility.OpenFolderPanel("Load Graph", Constants.GraphsRoot, "");

            if (String.IsNullOrWhiteSpace(fullPath))
                return;

            if (!GraphExists(fullPath))
                return;

            string relativePath = PathUtility.GetRelativePath(fullPath);

            if (DialogueGraphWindow.Windows.Any(window => window != DialogueGraphWindow.C && window.graphView.GraphPath == relativePath))
            {
                await DialogueGraphToolbar.C.ShowError("Graph open in another window");
                return;
            }

            LoadGraph(relativePath);
        }

        private void LoadGraph(string path)
        {
            newestNode = null;
            GraphPath = PathUtility.GetRelativePath(path);
            GraphName = Path.GetFileName(GraphPath);
            DialogueGraphWindow.C.SetTitle(GraphName);
            DialogueGraphToolbar.C.ClearStatus();
            Clear();
            this.Show();

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

        public async void TryCreateGraph()
        {
            string path = PathUtility.GetRelativePath(EditorUtility.OpenFolderPanel("Choose Folder for New Graph", Constants.GraphsRoot, ""));

            if (String.IsNullOrWhiteSpace(path))
                return;

            if (DialogueGraphWindow.Windows.Any(window => window != DialogueGraphWindow.C && window.graphView.GraphPath == path))
            {
                await DialogueGraphToolbar.C.ShowError("Graph open in another window");
                return;
            }

            if (GraphExists(path))
            {
                LoadGraph(path);
                await DialogueGraphToolbar.C.ShowWarning("Opening existing graph");
                return;
            }

            CreateGraph(path);
        }

        internal void CreateGraph(string path)
        {
            DialogueGraphToolbar.C.ClearStatus();

            Directory.CreateDirectory(path);
            Directory.CreateDirectory(Path.Combine(path, "Ungrouped"));
            Directory.CreateDirectory(Path.Combine(path, "Groups"));
            AssetDatabase.Refresh();
            LoadGraph(path);
        }

        public void CloseGraph()
        {
            DialogueGraphWindow.C.SetTitle("Dialogue Graph");
            DialogueGraphToolbar.C.ClearStatus();
            GraphName = GraphPath = "";
            Clear();
            this.Hide();
        }

        public void OnTextInputFocusIn(DialogueNode node)
        {
            ClearSelection();
            AddToSelection(node);
        }

        public void AddNode(int startingChoices = 0)
        {
            if (!this.Visible()) return;

            DialogueNode newNode = new(MousePosition, startingChoices);
            newestNode = newNode;
            AddElement(newNode);
        }

        public void AddConnectedNode()
        {
            if (!this.Visible()) return;
            
            DialogueNode newNode = new(MousePosition);
            AddElement(newNode);

            if (newestNode == null)
            {
                newestNode = newNode;
                return;
            }

            if (newestNode.TextNode)
                newestNode.ConnectTo(newNode);
            else
                newestNode.ChoicesDisplay.Children.First(display => !display.Output.connected).ConnectTo(newNode);
            
            newestNode = newNode;
        }

        public void AddGroup()
        {
            if (this.Visible())
                AddElement(new DialogueGroup(MousePosition));
        }

        private new void Clear()
        {
            foreach (var element in graphElements.OfType<IDialogueElement>())
                element.Remove();
        }

        private static bool GraphExists(string fullPath)
        {
            string ungroupedPath = PathUtility.Combine(fullPath, "Ungrouped");
            string groupsPath = PathUtility.Combine(fullPath, "Groups");
            return Directory.Exists(fullPath) && Directory.Exists(ungroupedPath) && Directory.Exists(groupsPath);
        }

        #region Menu
        public override void BuildContextualMenu(ContextualMenuPopulateEvent _) {}

        private void AddMenuItem(string title, Action<DropdownMenuAction> action) => this.AddManipulator(new ContextualMenuManipulator(menuEvent => menuEvent.menu.AppendAction(title, action)));
        #endregion

        #region Events
        private static void NodesAddedToGroup(Group group, IEnumerable<GraphElement> nodes)
        {
            var dialogueGroup = (DialogueGroup)group;

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

        private GraphViewChange DeleteSelected(GraphViewChange change)
        {
            if (change.elementsToRemove == null)
                return change;

            if (change.elementsToRemove.OfType<DialogueNode>().Any())
                newestNode = null;

            foreach (var element in change.elementsToRemove.ToArray().OfType<IDialogueElement>())
                element.Delete();

            return change;
        }
        #endregion
        #endregion
    }
}