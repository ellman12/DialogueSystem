using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DialogueSystem.Scripts
{
    ///A button displaying a choice in a conversation.
    public sealed class ChoiceButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private int index;

        [SerializeField]
        private TextMeshProUGUI text;

        public void Show()
        {
            text.text = ConversationManager.I.Current.Choices[index].Text;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            ConversationManager.I.SelectChoice(index);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            
        }
    }
}
