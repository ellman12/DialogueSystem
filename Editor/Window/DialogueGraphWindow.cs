using System;
using System.IO;
using System.Linq;
using DialogueSystem.Editor.Extensions;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Window
{
    public sealed class DialogueGraphWindow : EditorWindow
    {
        [MenuItem("Window/Dialogue Graph")]
        public static void Open() => CreateWindow<DialogueGraphWindow>("Dialogue Graph");

        internal DialogueGraphView graphView;
        public static DialogueGraphView GraphView => C.graphView;

        private DialogueGraphToolbar toolbar;
        public static DialogueGraphToolbar Toolbar => C.toolbar;

        public static DialogueGraphWindow C { get; private set; }

        public static IEnumerable<DialogueGraphWindow> Windows => Resources.FindObjectsOfTypeAll<DialogueGraphWindow>();

        private static readonly Dictionary<(EventModifiers, KeyCode), Action> hotkeys = new()
        {
            {(EventModifiers.Control, KeyCode.N), () => DialogueGraphView.C.AddNode()},
            {(EventModifiers.Alt, KeyCode.N), () => DialogueGraphView.C.AddConnectedNode()},
            {(EventModifiers.Control | EventModifiers.Shift, KeyCode.N), () => DialogueGraphView.C.AddNode(2)},
            {(EventModifiers.Control, KeyCode.G), () => DialogueGraphView.C.AddGroup()},
            {(EventModifiers.Control, KeyCode.O), () => DialogueGraphView.C.TryLoadGraph()},
            {(EventModifiers.Control | EventModifiers.Alt | EventModifiers.Shift, KeyCode.N), () => DialogueGraphView.C.TryCreateGraph()},
            {(EventModifiers.Control, KeyCode.W), CtrlW},
            {(EventModifiers.Control, KeyCode.P), DialogueGraphToolbar.Ping}
        };

        private static readonly HashSet<(EventModifiers, KeyCode)> validDefaultHotkeys = new() {(EventModifiers.FunctionKey, KeyCode.Delete)};

        public void SetTitle(string newTitle) => titleContent.text = newTitle;

        private void OnFocus() => C = this;

        private void CreateGUI()
        {
            C = this;

            rootVisualElement.AddStyleSheet("Constants");

            VisualElement container = new() {focusable = true};
            container.StretchToParentSize();
            container.RegisterCallback<KeyDownEvent>(HandleHotkeys);
            rootVisualElement.Add(container);

            graphView = new DialogueGraphView();
            graphView.Hide();
            container.Add(graphView);

            toolbar = new DialogueGraphToolbar();
            container.Add(toolbar);

            Directory.CreateDirectory(Constants.GraphsRoot);
            AssetDatabase.Refresh();
        }

        private static void HandleHotkeys(KeyDownEvent e)
        {
            if (validDefaultHotkeys.Contains((e.modifiers, e.keyCode)))
                return;

            e.PreventDefault();
            e.StopPropagation();
            e.StopImmediatePropagation();

            if (hotkeys.TryGetValue((e.modifiers, e.keyCode), out Action action))
                action();
        }

        private static void CtrlW()
        {
            if (DialogueGraphView.C.GraphOpen)
                DialogueGraphView.C.CloseGraph();
            else
                C.Close();
        }
    }
}