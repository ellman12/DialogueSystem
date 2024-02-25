using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogueSystem.Editor.Elements
{
    public sealed class DialogueGroup : Group
    {
        public Guid Id = Guid.NewGuid();
        public Vector2 Position;
        
        public DialogueGroup(Vector2 position)
        {
            title = "New Group";
            Position = position;
            SetPosition(new Rect(position, Vector2.zero));
        }
    }
}