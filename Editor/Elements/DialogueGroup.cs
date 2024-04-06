using System;
using System.Linq;
using DialogueSystem.Data;
using DialogueSystem.Editor.Elements.Interfaces;
using DialogueSystem.Editor.Utilities;
using DialogueSystem.Editor.Window;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor.Elements
{
    [Serializable]
    public sealed class DialogueGroup : Group, ISaveableElement<GroupSaveData>
    {
        public GroupSaveData SaveData { get; }

        #region Constructors
        public DialogueGroup(Vector2 position)
        {
            SaveData = GroupSaveData.Create(position);

            RegisterCallback<FocusOutEvent>(_ => FocusOut());

            title = SaveData.Id;
            SetPosition(new Rect(position, Vector2.zero));
        }

        public DialogueGroup(GroupSaveData saveData)
        {
            SaveData = saveData;

            RegisterCallback<FocusOutEvent>(_ => FocusOut());

            title = SaveData.Name;
            SetPosition(new Rect(SaveData.Position, Vector2.zero));
        }
        
        private async void FocusOut()
        {
            string newName = title.Trim().RemoveInvalidChars();

            if (DialogueGraphView.C.graphElements.OfType<ISaveableElement<SaveData>>().Where(element => element != this).Any(element => element.SaveData.Name == newName))
            {
                DialogueGraphToolbar.C.Error.text = "Name in use";
                SaveData.Name = "";
                await Task.Delay(3000);
                DialogueGraphToolbar.C.Error.text = "";
            }
            else
                SaveData.Name = newName;

            SaveData.Save();
            title = SaveData.Name;
        }
        #endregion

        public void UpdatePosition(Vector2 newPosition)
        {
            SaveData.Position = newPosition;
            SaveData.Save();

            foreach (var node in containedElements.Cast<DialogueNode>())
                node.UpdatePosition(node.GetPosition().position);
        }

        public void Remove() => DialogueGraphView.C.RemoveElement(this);

        public void Delete()
        {
            Remove();
            SaveData.Delete();
        }
    }
}