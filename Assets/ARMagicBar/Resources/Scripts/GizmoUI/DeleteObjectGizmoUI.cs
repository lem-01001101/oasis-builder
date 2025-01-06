using UnityEngine;

namespace ARMagicBar.Resources.Scripts.GizmoUI
{
    public class DeleteObjectGizmoUI : MonoBehaviour
    {
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