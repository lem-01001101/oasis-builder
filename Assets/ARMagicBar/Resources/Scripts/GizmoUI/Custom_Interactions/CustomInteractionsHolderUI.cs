using System.Collections.Generic;
using UnityEngine;

namespace ARMagicBar.Resources.Scripts.GizmoUI.Custom_Interactions
{
    public class CustomInteractionsHolderUI : MonoBehaviour
    {
        private List<CustomInteractionUI> _customInteractionUis = new();
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}