using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Common.Controller
{
    public class ClickableControl : MonoBehaviour, IPointerClickHandler
    {
        public event Action OnClick;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }
    }
}