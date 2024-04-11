using DialogueSystem.Data;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DialogueSystem.Scripts
{
    ///Manages the current conversation and all the relevant UI elements.
    public sealed class ConversationManager : Singleton<ConversationManager>, IPointerClickHandler
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

                StartCoroutine(TypeLine());
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
            if (index < 1 || index >= Current.Choices.Count + 1)
                throw new ArgumentOutOfRangeException();

            OnChoiceSelected?.Invoke(null, EventArgs.Empty);
            Current = Current.Choices[index - 1].Node;
        }

        ///Finish the conversation by setting Current to null.
        public void Finish()
        {
            OnFinish?.Invoke(null, EventArgs.Empty);
            Current = null;
        }

        private void SetupChoices()
        {
            if (Current == null)
                return;

            for (int i = 0; i < current.Choices.Count; i++)
                choiceButtons[i].Show();
        }

        private void HideButtons()
        {
            foreach (var button in choiceButtons)
                button.Hide();
        }

        private IEnumerator TypeLine()
        {
            dialogueText.text = "";
            HideButtons();

            foreach (char c in Current.Text)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(textSpeed);
            }

            SetupChoices();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Advance();
        }
    }
}