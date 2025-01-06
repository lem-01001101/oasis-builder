using System.Collections.Generic;
using UnityEngine;

namespace ARMagicBar.Resources.Scripts.GizmoUI.Custom_Interactions
{
    [CreateAssetMenu(menuName = "ARMagicBar/CustomInteractionListSO")]
    public class CustomInteractionListSO : ScriptableObject
    {
        [SerializeField] public List<CustomInteractionDataSO> _customInteractionDataSos;
    }
}