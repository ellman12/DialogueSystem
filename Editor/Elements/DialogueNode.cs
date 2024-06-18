using DialogueSystem.Data;
using DialogueSystem.Editor.Elements.Interfaces;
using DialogueSystem.Editor.Extensions;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Editor.Window;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Elements
{
    public sealed class DialogueNode : Node, ISaveableElement<NodeSaveData>
    {
        public Port Input { get; private set; }
        public Port Output { get; private set; }

        public NodeSaveData SaveData { get; }

        private TextField nameTextField;

        public ChoicesDisplay ChoicesDisplay { get; private set; }

        public bool TextNode => SaveData.Type == NodeType.Text;

        public bool PromptNode => SaveData.Type == NodeType.Prompt;

        #region Constructors
        public DialogueNode(Vector2 position, int startingChoices = 0)
        {
            SaveData = NodeSaveData.Create(position);
            SaveData.Node = this;

            RegisterCallback<FocusOutEvent>(_ => FocusOut());

            AddElements();

            for (int i = 0; i < startingChoices; i++)
                ChoicesDisplay.Add();
        }

        public DialogueNode(NodeSaveData saveData)
        {
            SaveData = saveData;

            RegisterCallback<FocusOutEvent>(_ => FocusOut());

            AddElements();

            foreach (var choice in SaveData.Choices)
                ChoicesDisplay.Add(choice);
        }

        private void AddElements()
        {
            SetPosition(new Rect(SaveData.Position, Vector2.zero));
            this.AddStyleSheet("Nodes/DialogueNode");

            ChoicesDisplay = new ChoicesDisplay(this);

            #region Title Container
            Input = ElementExtensions.CreatePort(Direction.Input, Port.Capacity.Multi);
            titleButtonContainer.Insert(0, Input);

            nameTextField = ElementExtensions.CreateTextField(_ => {}, SaveData.Name == SaveData.Id ? "" : SaveData.Name);
            nameTextField.RegisterCallback<FocusInEvent>(_ => DialogueGraphView.C.OnTextInputFocusIn(this));
            titleButtonContainer.Insert(1, nameTextField);

            titleButtonContainer.InsertIconButton(2, "Add", ChoicesDisplay.Add);

            Output = ElementExtensions.CreatePort(Direction.Output, Port.Capacity.Single);
            titleButtonContainer.Add(Output);
            #endregion

            #region Extension Container
            var characterTextField = ElementExtensions.CreateTextField(e => SaveData.Character = e.newValue, SaveData.Character);
            characterTextField.RegisterCallback<FocusInEvent>(_ => DialogueGraphView.C.OnTextInputFocusIn(this));
            extensionContainer.Add(characterTextField);

            var textTextField = ElementExtensions.CreateTextField(e => SaveData.Text = e.newValue, SaveData.Text, true);
            textTextField.RegisterCallback<FocusInEvent>(_ => DialogueGraphView.C.OnTextInputFocusIn(this));
            extensionContainer.Add(textTextField);

            extensionContainer.Add(ChoicesDisplay);

            expanded = true;
            RefreshExpandedState();
            #endregion
        }
        #endregion

        public void UpdatePosition(Vector2 newPosition)
        {
            SaveData.Position = newPosition;
            SaveData.Save();
        }

        public void ConnectTo(DialogueNode node)
        {
            DialogueGraphView.C.AddElement(Output.ConnectTo<DialogueEdge>(node.Input));
            SaveData.Next = node.SaveData;
            SaveData.Save();
        }

        public void Remove() => DialogueGraphView.C.RemoveElement(this);

        public void Delete()
        {
            DisconnectAllPorts();
            Remove();
            SaveData.Delete();
        }

        private async void FocusOut()
        {
            string newName = nameTextField.value.Trim().RemoveInvalidChars();

            if (DialogueGraphView.C.graphElements.OfType<ISaveableElement<SaveData>>().Where(element => element != this).Any(element => element.SaveData.Name == newName))
            {
                await DialogueGraphToolbar.C.ShowError("Name in use");
                nameTextField.value = SaveData.Name = "";
            }
            else
                SaveData.Name = newName;

            SaveData.Save();
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

            DialogueGraphView.C.DeleteElements(Output.connections);
            Output.style.display = DisplayStyle.None;
        }

        private void DisconnectAllPorts()
        {
            DisconnectInputPort();
            DisconnectOutputPorts();
        }

        private void DisconnectInputPort() => DialogueGraphView.C.DeleteElements(Input.connections);
        private void DisconnectOutputPorts()
        {
            SaveData.Next = null;
            SaveData.Save();

            DialogueGraphView.C.DeleteElements(Output.connections);

            foreach (var choiceDisplay in ChoicesDisplay.Children)
                choiceDisplay.DisconnectPort();
        }
        #endregion
    }
}