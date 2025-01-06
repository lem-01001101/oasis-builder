using ARMagicBar.Resources.Scripts.Debugging;
using ARMagicBar.Resources.Scripts.GizmoUI.Custom_Interactions;
using ARMagicBar.Resources.Scripts.PlacementObjects;
using ARMagicBar.Resources.Scripts.TransformLogic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMagicBar.Resources.Scripts.ExampleExtension.Custom_Interactions
{
    public class CustomInteractionPickUpExample : MonoBehaviour
    {
        [SerializeField] private CustomInteractionDataSO pickUpInteractionSo;
        private ARCameraManager _cameraManager;

        private bool shouldPickUpObject = false;
        
        private void Start()
        {
            CustomInteractionUI.OnCustomInteractionTriggered += PickUpItemOnTriggered;
            _cameraManager = FindObjectOfType<ARCameraManager>();
        }

        private void PickUpItemOnTriggered((string nameOfInteraction, 
            GameObject referenceToObject, 
            ReferenceToSO referenceToSo, 
            CustomInteractionDataSO customInteractionDataSo) obj)
        {
            if (obj.customInteractionDataSo == pickUpInteractionSo)
            {
                CustomLog.Instance.InfoLog("Picking Up Object");
                shouldPickUpObject = true;
                SelectObjectsLogic.Instance.DeselectAllObjects();
            }
        }
        
        private void Update()
        {
            if (shouldPickUpObject)
            {
                HoldObjectAtCameraPosition();
            }

            if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)) && shouldPickUpObject)
            {
                shouldPickUpObject = false;
            }
        }
        
        private void HoldObjectAtCameraPosition()
        {
            transform.position = _cameraManager.transform.position - new Vector3(0,0,-1);
        }
    }
}