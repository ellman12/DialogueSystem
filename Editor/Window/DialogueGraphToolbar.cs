using DialogueSystem.Data;
using DialogueSystem.Editor.Elements.Interfaces;
using System;
using System.IO;
using DialogueSystem.Editor.Extensions;
using DialogueSystem.Editor.Utilities;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Window
{
    public sealed class DialogueGraphToolbar : Toolbar
    {
        public static DialogueGraphToolbar C => DialogueGraphWindow.Toolbar;

        private readonly Label status = new();

        public DialogueGraphToolbar()
        {
            this.AddStyleSheet("Toolbar");

            this.AddButton("Close", DialogueGraphView.C.CloseGraph);
            this.AddButton("Load", DialogueGraphView.C.TryLoadGraph);
            this.AddButton("Create", DialogueGraphView.C.TryCreateGraph);
            this.AddButton("Ping", Ping);

            Add(status);
        }

        public void ClearStatus() => status.text = "";

        public async Task ShowWarning(string message) => await ShowMessage(message, Color.yellow);

        public async Task ShowError(string message) => await ShowMessage(message, Color.red);

        public async Task ShowMessage(string message, Color color)
        {
            status.style.color = color;
            status.text = message;
            await Task.Delay(3000);
            status.text = "";
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