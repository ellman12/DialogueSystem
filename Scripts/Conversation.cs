using System;
using DialogueSystem.Data;
using UnityEngine;

namespace DialogueSystem.Scripts
{
	///Manages a conversation with an NPC.
	public sealed class Conversation : MonoBehaviour
    {
        private NodeSaveData current;
        public NodeSaveData Current
        {
            get => current;
            set
            {
                current = value;
                OnCurrentChanged?.Invoke(null, EventArgs.Empty);
            }
        }

		public event EventHandler OnCurrentChanged, OnBegin, OnAdvance, OnChoiceSelected, OnEnd;

		[SerializeField]
		private NodeSaveData start;

		///Start the conversation by setting Current to the start node.
		public void Begin()
		{
			OnBegin?.Invoke(null, EventArgs.Empty);
			Current = start;
		}

		///Advance to the next text node.
		public void Advance()
		{
			OnAdvance?.Invoke(null, EventArgs.Empty);
			
			if (Current.Next != null)
				Current = Current.Next;
		}

		///Advances the conversation to the selected choice node. Indexes are 1-based.
		public void SelectChoice(int index)
		{
			if (index < 1 || index >= Current.Choices.Count + 1) return;
			
			OnChoiceSelected?.Invoke(null, EventArgs.Empty);
			Current = Current.Choices[index - 1].Node;
		}

		///Finish the conversation by setting Current to null.
		public void End()
		{
			OnEnd?.Invoke(null, EventArgs.Empty);
			Current = null;
		}
	}
}