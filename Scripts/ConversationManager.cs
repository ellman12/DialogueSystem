using DialogueSystem.Data;
using System;
using TMPro;
using UnityEngine;

namespace DialogueSystem.Scripts
{
    ///Manages the current conversation and all the relevant UI elements.
    public sealed class ConversationManager : Singleton<ConversationManager>
    {
        [SerializeField]
        private TextMeshProUGUI nameText, dialogueText;
        
        [SerializeField]
        private ChoiceButton[] choiceButtons;

        [SerializeField]
        private float textSpeed;

        private NodeSaveData current;
        public NodeSaveData Current
        {
            get => current;
            set
            {
                OnCurrentChanged?.Invoke(null, EventArgs.Empty);
                current = value;
                dialogueText.text = current.Text;

                HideButtons();

                if (value == null) return;
                
                for (int i = 0; i < current.Choices.Count; i++)
                    choiceButtons[i].gameObject.SetActive(true);
            }
        }

        public event EventHandler OnCurrentChanged, OnBegin, OnAdvance, OnChoiceSelected, OnFinish;

        public bool OnFinalNode => Current.Next == null && Current.Choices.Count == 0;

        private void Start()
        {
            HideButtons();
        }

        ///Start the conversation by setting Current to the start node.
        public void Begin(NodeSaveData start)
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
        public void Finish()
        {
            OnFinish?.Invoke(null, EventArgs.Empty);
            Current = null;
        }

        private void HideButtons()
        {
            foreach (var button in choiceButtons)
                button.gameObject.SetActive(false);
        }
    }
}