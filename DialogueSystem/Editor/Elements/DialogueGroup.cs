using System;
using DialogueSystem.Data;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogueSystem.Editor.Elements
{
    [Serializable]
    public sealed class DialogueGroup : Group
    {
        public readonly GroupSaveData SaveData;
        
        public DialogueGroup(Vector2 position)
        {
            SaveData = GroupSaveData.Create(position);
            title = SaveData.Id;
            SetPosition(new Rect(position, Vector2.zero));
        }
    }
}