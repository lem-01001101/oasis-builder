using System;
using System.Collections.Generic;
using ARMagicBar.Resources.Scripts.Debugging;
using ARMagicBar.Resources.Scripts.PlacementObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace ARMagicBar.Resources.Scripts.GizmoUI.Custom_Interactions
{
    [RequireComponent(typeof(ReferenceToSO))]
    public class CustomInteractionManager : MonoBehaviour
    {
        [Header("Add your custom interactions below. Buttons will be generated on Start.")]
        [SerializeField] private CustomInteractionListSO _customInteractionListSo;
        [Header("Subscribe to CustomInteractionUI.OnCustomInteractionTriggered to get information on the interaction type")]
        
        [HideInInspector]
        [SerializeField] private CustomInteractionUI _customInteractionUI;
        [HideInInspector]
        [SerializeField] private Transform customUIParent;

        private ReferenceToSO _referenceToSo;
        private List<CustomInteractionUI> customInteractions = new();

        public int GetAmountOfCustomInteractions()
        {
            if (_customInteractionListSo == null) return 0;
            
            return _customInteractionListSo._customInteractionDataSos.Count;
        }

        private void Start()
        {
            _referenceToSo = GetComponent<ReferenceToSO>();
            InstantiateCustomInteractions();
            
            CustomInteractionUI.OnCustomInteractionTriggered += OnCustomInteractionTriggered;
        }

        private void OnCustomInteractionTriggered((string nameOfInteraction, GameObject referenceToObject, ReferenceToSO referenceToSo, CustomInteractionDataSO customInteractionDataSo) obj)
        {
            CustomLog.Instance.InfoLog($"Custom interaction was triggered with the attributes name {obj.nameOfInteraction}, " +
                                       $"Object: {obj.referenceToObject} " +
                                       $" , RefToSO: {obj.referenceToSo} and Data: ${obj.customInteractionDataSo}");
        }

        void InstantiateCustomInteractions()
        {
            if(_customInteractionListSo == null) return;
            
            foreach (var customInteraction in _customInteractionListSo._customInteractionDataSos)
            {
                CustomInteractionUI interactionUI = Instantiate(customInteraction.customUIPrefab, parent:customUIParent);
                interactionUI.SetImage(customInteraction.icon);
                interactionUI.SetAttributes(customInteraction.nameOfInteraction, transform.gameObject, _referenceToSo, customInteraction);
                customInteractions.Add(interactionUI);
            }
        }
        
    }
}