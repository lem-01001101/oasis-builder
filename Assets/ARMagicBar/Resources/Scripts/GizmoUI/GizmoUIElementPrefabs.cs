using UnityEngine;

namespace ARMagicBar.Resources.Scripts.GizmoUI
{
    public class GizmoUIElementPrefabs : MonoBehaviour
    {
        [Header("Double click to change the elements (Will only affect new objects)")]
        [SerializeField] private GameObject MoveElement;
        [SerializeField] private GameObject ScaleElement;
        [SerializeField] private GameObject RotateElement;
        [SerializeField] private GameObject ReturnElement;
        [SerializeField] private GameObject ResetElement;
        [SerializeField] private GameObject DeleteElement;
        [SerializeField] private GameObject CustomInteraction; 
    }
}
