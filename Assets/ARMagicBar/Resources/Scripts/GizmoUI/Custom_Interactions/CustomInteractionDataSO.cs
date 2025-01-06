using UnityEngine;
using UnityEngine.UI;

namespace ARMagicBar.Resources.Scripts.GizmoUI.Custom_Interactions
{
    [CreateAssetMenu(menuName = "ARMagicBar/CustomInteraction")]
    public class CustomInteractionDataSO : ScriptableObject
    {
        [SerializeField] public string nameOfInteraction;
        [SerializeField] public Texture2D icon;
        [SerializeField] public CustomInteractionUI customUIPrefab; 
    }
}