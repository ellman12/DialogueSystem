using DialogueSystem.Data;
using DialogueSystem.Editor.Elements.Interfaces;
using System;
using System.IO;
using DialogueSystem.Editor.Extensions;
using DialogueSystem.Editor.Utilities;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Window
{
    public sealed class DialogueGraphToolbar : Toolbar
    {
        public Label Error { get; } = new();

        public static DialogueGraphToolbar C => DialogueGraphWindow.Toolbar;

        public DialogueGraphToolbar()
        {
            this.AddStyleSheet("Toolbar");

            this.AddButton("Close", DialogueGraphView.C.CloseGraph);
            this.AddButton("Load", DialogueGraphView.C.TryLoadGraph);
            this.AddButton("Create", DialogueGraphView.C.TryCreateGraph);
            this.AddButton("Ping", Ping);

            Error.style.color = Color.red;
            Add(Error);
        }

        private static void Ping()
        {
            var selection = DialogueGraphView.C.selection;

            if (selection.Count == 0)
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(DialogueGraphView.C.GraphPath));
            else if (selection.Count == 1 && selection.First() is ISaveableElement<SaveData> element)
                EditorGUIUtility.PingObject(element.SaveData);
        }
    }
}