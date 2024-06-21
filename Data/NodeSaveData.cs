using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DialogueSystem.Editor.Elements;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Editor.Window;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Data
{
    public sealed class NodeSaveData : SaveData
    {
        public DialogueNode Node { get; set; }

        public NodeType Type => Choices.Count == 0 ? NodeType.Text : NodeType.Prompt;

        public override string Name
        {
            get => name;
            set
            {
                string newName = String.IsNullOrWhiteSpace(value) ? Id : value.Trim();
                UpdatePaths(newName);

                if (!File.Exists(path) && !File.Exists(previousPath))
                {
                    AssetDatabase.CreateAsset(this, path);
                }
                else if (previousName != newName)
                {
                    //Setting name directly does not work. RenameAsset() sets name for us.
                    AssetDatabase.RenameAsset(previousPath, newName);
                    OnReinitialize();
                }
                else if (previousPath != path)
                {
                    AssetDatabase.MoveAsset(previousPath, path);
                }
            }
        }

        public string Text = "Text";

        public string Character = "Character Name";
        
        public string VoiceLineFilename = "";

        public AnimationClip AnimationClip;

        public NodeSaveData Next;
        public List<ChoiceSaveData> Choices = new();

        [SerializeField]
        private GroupSaveData group;
        public GroupSaveData Group
        {
            get => group;
            set
            {
                group = value;
                UpdatePaths(name);

                AssetDatabase.MoveAsset(previousPath, path);
            }
        }

        public static NodeSaveData Create(Vector2 position) => SaveData.Create<NodeSaveData>(position);

        public override void Delete() => AssetDatabase.DeleteAsset(path);

        protected override void UpdatePaths(string newName)
        {
            previousName = name;
            previousPath = path;

            folderPath = PathUtility.Combine(DialogueGraphView.C.GraphPath, Group == null ? "Ungrouped" : $"Groups/{Group.Name}");
            path = PathUtility.Combine(folderPath, $"{newName}.asset");
        }

        ///When the name of a SO changes, Unity reinitializes the SO and copies the values of its members. Because of this, references to reference types (in this case the choices) become stale. I hate this and I hate Unity.
        private void OnReinitialize()
        {
            if (Node?.ChoicesDisplay == null || Choices.Count == 0)
                return;

            var children = Node.ChoicesDisplay.Children.ToArray();
            for (int i = 0; i < children.Length; i++)
                children[i].SaveData = Choices[i];
        }
    }
}