using UnityEngine;
using UnityEngine.EventSystems;

namespace DialogueSystem.Scripts
{
    ///A button displaying a choice in a conversation.
    public sealed class ChoiceButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public int index;
        
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
