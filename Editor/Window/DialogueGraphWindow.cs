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

        private static readonly Dictionary<(EventModifiers, KeyCode), Action> shortcuts = new()
        {
            {(EventModifiers.Control, KeyCode.N), () => DialogueGraphView.C.AddNode()},
            {(EventModifiers.Alt, KeyCode.N), () => DialogueGraphView.C.AddNode(2)},
            {(EventModifiers.Control, KeyCode.O), () => DialogueGraphView.C.TryLoadGraph()},
            {(EventModifiers.Control | EventModifiers.Shift, KeyCode.N), () => DialogueGraphView.C.TryCreateGraph()}
        };

        //TODO: delete this later
        [MenuItem("DS/Clear &c")]
        public static void Clear(MenuCommand _)
        {
            Directory.Delete(Constants.GraphsRoot, true);
            Directory.CreateDirectory(Constants.GraphsRoot);
            AssetDatabase.Refresh();
        }

        public void SetTitle(string newTitle) => titleContent.text = newTitle;

        private void OnFocus() => C = this;

        private void CreateGUI()
        {
            C = this;

            rootVisualElement.AddStyleSheet("Constants");

            graphView = new DialogueGraphView();
            graphView.Hide();
            rootVisualElement.Add(graphView);

            toolbar = new DialogueGraphToolbar();
            rootVisualElement.Add(toolbar);

            rootVisualElement.RegisterCallback<KeyDownEvent>(HandleKeyboardShortcuts);

            Directory.CreateDirectory(Constants.GraphsRoot);
            AssetDatabase.Refresh();

            //TODO: remove this later
			// string path = PathUtility.Combine(Environment.CurrentDirectory, Constants.GraphsRoot, Guid.NewGuid().ToString()).Replace('\\', '/');
			// graphView.CreateGraph(path);
        }

        private static void HandleKeyboardShortcuts(KeyDownEvent e)
        {
            e.PreventDefault();
            e.StopPropagation();
            e.StopImmediatePropagation();

            if (shortcuts.TryGetValue((e.modifiers, e.keyCode), out Action action))
                action();
        }
    }
}