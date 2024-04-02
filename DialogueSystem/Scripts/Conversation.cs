using System;
using DialogueSystem.Data;
using UnityEngine;

namespace DialogueSystem.Scripts
{
	///Manages a conversation with an NPC.
	public sealed class Conversation : MonoBehaviour
	{
		public NodeSaveData Current { get; private set; }

		public event EventHandler OnBegin, OnAdvance, OnEnd;

		[SerializeField]
		private NodeSaveData start;

		public void Begin()
		{
			OnBegin?.Invoke(null, EventArgs.Empty);
			Current = start;
		}

		public void Advance()
		{
			OnAdvance?.Invoke(null, EventArgs.Empty);
			
			if (Current.Next != null)
				Current = Current.Next;
		}

		public void End()
		{
			OnEnd?.Invoke(null, EventArgs.Empty);
			Current = null;
		}
	}
}