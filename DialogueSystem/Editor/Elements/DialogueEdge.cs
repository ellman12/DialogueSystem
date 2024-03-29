using DialogueSystem.Data;
using DialogueSystem.Editor.Elements.Interfaces;
using DialogueSystem.Editor.Window;
using UnityEditor.Experimental.GraphView;

namespace DialogueSystem.Editor.Elements
{
	///Connects dialogue nodes together to form conversations.
	public sealed class DialogueEdge : Edge, IDialogueElement
	{
		public DialogueNode StartNode => (DialogueNode) output.node;
		public DialogueNode EndNode => (DialogueNode) input.node;

		public void Remove()
		{
			input.Disconnect(this);
			output.Disconnect(this);

			StartNode.SaveData.Next = null;

			if (output.userData is ChoiceSaveData choiceData)
				choiceData.Node = null;

			StartNode.SaveData.Save();
			DialogueGraphView.C.RemoveElement(this);
		}

		public void Delete() => Remove();
	}
}