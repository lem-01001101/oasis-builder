using UnityEngine;

namespace ARMagicBar.Resources.Scripts.GizmoUI
{
    public class ResetTransformGizmoUI : MonoBehaviour, IGizmos
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