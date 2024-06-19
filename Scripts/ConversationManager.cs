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

                if (value != null)
                    StartTypingLine();
            }
        }

        public event EventHandler OnCurrentChanged, OnBegin, OnAdvance, OnChoiceSelected, OnFinish;

        public bool OnFinalNode => Current.Next == null && Current.Choices.Count == 0;

        private Coroutine typingCoroutine;

        private void Start()
        {
            HideButtons();
            gameObject.SetActive(false);
        }

        ///Start the conversation by setting Current to the start node.
        public void Begin(NodeSaveData start)
        {
            OnBegin?.Invoke(null, EventArgs.Empty);
            gameObject.SetActive(true);
            Current = start;
        }

        ///Advance to the next text node.
        public void Advance()
        {
            OnAdvance?.Invoke(null, EventArgs.Empty);

            if (OnFinalNode)
                Finish();
            else if (Current.Next != null)
                Current = Current.Next;
        }

        ///Advances the conversation to the selected choice node. Indexes are 0-based.
        public void SelectChoice(int index)
        {
            if (index < 0 || index >= Current.Choices.Count)
                throw new ArgumentOutOfRangeException();

            OnChoiceSelected?.Invoke(null, EventArgs.Empty);
            Current = Current.Choices[index].Node;
        }

        ///Finish the conversation by setting Current to null.
        public void Finish()
        {
            OnFinish?.Invoke(null, EventArgs.Empty);
            gameObject.SetActive(false);
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

        private void StartTypingLine()
        {
            typingCoroutine = StartCoroutine(TypeLine());
        }

        private void FinishTypingLine()
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            dialogueText.text = Current.Text;
            SetupChoices();
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
            typingCoroutine = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (typingCoroutine == null)
                Advance();
            else
                FinishTypingLine();
        }
    }
}