using DialogueSystem.Editor.Elements.Interfaces;
using DialogueSystem.Editor.Window;
using UnityEditor.Experimental.GraphView;

namespace DialogueSystem.Editor.Elements
{
	///Connects dialogue nodes together to form conversations.
	public sealed class DialogueEdge : Edge, IDialogueElement
	{
		public void Remove()
		{
			input.Disconnect(this);
			output.Disconnect(this);
			DialogueGraphView.C.RemoveElement(this);
		}
		
        public DialogueNode StartNode => (DialogueNode)output.node;
        public DialogueNode EndNode => (DialogueNode)input.node;
	}
}