using ARMagicBar.Resources.Scripts.GizmoUI.Custom_Interactions;
using ARMagicBar.Resources.Scripts.PlacementObjects;
using ARMagicBar.Resources.Scripts.TransformLogic;
using UnityEngine;

namespace ARMagicBar.Resources.Scripts.ExampleExtension.Custom_Interactions
{
    //This can be added to a transformable object to trigger a particle effect
    public class CustomInteractionParticleEffectExample : MonoBehaviour
    {
        [SerializeField] private GameObject particleEffect;
        [Header("Drag in the interaction that should trigger the effect")]
        [SerializeField] private CustomInteractionDataSO levelUpInteractions;
        [Header("Drag in the _placeable object")]
        [SerializeField] private TransformableObject _transformableObject;
        private void Start()
        {
            CustomInteractionUI.OnCustomInteractionTriggered += CustomInteractionUIOnOnCustomInteractionTriggered;
        }

        private void CustomInteractionUIOnOnCustomInteractionTriggered((string nameOfInteraction, 
            GameObject referenceToObject, ReferenceToSO referenceToSo, CustomInteractionDataSO customInteractionDataSo) interactionData)
        {
            if (interactionData.referenceToObject == this.gameObject)
            {
                Debug.Log($"Interaction of type {interactionData.nameOfInteraction} " +
                          $"was called on object {gameObject.name}");

                if (interactionData.customInteractionDataSo == levelUpInteractions)
                {
                    Instantiate(particleEffect, _transformableObject.HighestPoint(), Quaternion.LookRotation(Vector3.up));
                }
                
            }
        }
    }
}