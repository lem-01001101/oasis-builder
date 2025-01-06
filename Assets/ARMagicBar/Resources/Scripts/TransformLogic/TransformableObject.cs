using System;
using ARMagicBar.Resources.Scripts.Debugging;
using ARMagicBar.Resources.Scripts.GizmoUI;
using ARMagicBar.Resources.Scripts.PlacementBar;
using ARMagicBar.Resources.Scripts.PlacementObjects;
using ARMagicBar.Resources.Scripts.UIIndicator;
using UnityEngine;
using UnityEngine.Serialization;

namespace ARMagicBar.Resources.Scripts.TransformLogic
{
    
    //Sits on each object that is supposed to be transformable 
    //Controls the visual 
    public class TransformableObject : MonoBehaviour
    {
        // [SerializeField] private GameObject SelectedVisual;
        [SerializeField] private GameObject TransformableObjectReference;
        [SerializeField] private ReferenceToSO _referenceToSo;
        
        private GameObject Visual;

        private Vector3 visualOriginalTransformPosition;
        private Quaternion visualOriginalTransformRotation; 
        private Vector3 visualOriginalTransformScale;

        
        private Vector3 selectedVisualOriginalTransformPosition;
        private Quaternion selectedVisualOriginalTransformRotation; 
        private Vector3 selectedVisualOriginalTransformScale;

        private Vector3 uiOriginalTransformPosition; 
        private Quaternion uiOriginalTransformRotation; 
        private Vector3 uiOriginalTransformScale;

        private bool canObjectBeSelected = true;

        //Set if the object can be selected
        public bool CanObjectBeSelected
        {
            get => canObjectBeSelected;
            set => canObjectBeSelected = value;
        }
        
        private bool IsSelected;
        
        //Element was selected
        public event Action<bool> OnWasSelected;
        
        
        //Element is being deleted
        public static event Action<GameObject> OnBeingDeleted;

        private ControlPosition_TransformSelector_UI_Indicator uiPosition;

        public PlacementObjectSO GetCorrespondingPlacementObject
        {
            get => _referenceToSo.correspondingObject;
        }

        public event Action OnWasMoved;

        public Vector3 HighestPoint()
        {
            if (uiPosition)
            {
                return uiPosition.topPositionOfObject;
            }
            else
            {
                CustomLog.Instance.ErrorLog("Did not find the uiPositionScript in children");
                return Vector3.zero;
            }
        }
        
        private void OnEnable()
        {
            TransformableObjectReference = this.transform.gameObject;
            Visual = GetVisual().gameObject;

            if (Visual == null)
            {
                Debug.LogError("Placeable Object has no Transform attached");
                return;
            }
            visualOriginalTransformPosition = Visual.transform.localPosition;
            visualOriginalTransformRotation = Visual.transform.localRotation;
            visualOriginalTransformScale = Visual.transform.localScale;
            uiPosition = GetComponentInChildren<ControlPosition_TransformSelector_UI_Indicator>();
        }

        UnityEngine.Transform GetVisual()
        {
            UnityEngine.Transform[] childObjects = GetComponentsInChildren<UnityEngine.Transform>();
            foreach (var transf in childObjects)
            {
                if (transf.GetComponent<PlacementObjectVisual.PlacementObjectVisual>())
                {
                    return transf;
                }
            }
            return null;
        }

        public void ResetScale()
        {
            Visual.transform.localScale = visualOriginalTransformScale;
        }

        public void ResetObject()
        {
            Visual.transform.localPosition = visualOriginalTransformPosition;
            Visual.transform.localRotation = visualOriginalTransformRotation;
            // SelectedVisual.transform.position = Visual.transform.localPosition;
            // SelectedVisual.transform.rotation = Visual.transform.localRotation;
            // SelectedVisual.transform.localScale = Visual.transform.localScale;
        }

        private void Start()
        {
            GizmoHolderUI.Instance.deleteButtonToggled += Delete;
        }

        private void OnDestroy()
        {
            GizmoHolderUI.Instance.deleteButtonToggled -= Delete;
        }

        public void MoveVisual(Vector3 movementVector)
        {
            CustomLog.Instance.InfoLog("Should move visual " + gameObject.name);
            transform.position += movementVector;
        }


        void AdjustSelf()
        {
        }

        public void RotateObject(Quaternion rotation)
        {

            // For the Visual object
            Visual.transform.Rotate(rotation.eulerAngles, Space.World); // Correctly apply the rotation

            // For the SelectedVisual object
            // SelectedVisual.transform.rotation *= rotationQuart; // Correctly apply the rotation
        }

        public void ScaleObject(Vector3 deltaScale)
        {
            // Apply delta scale to the current scale. This ensures that the scaling is incremental.
            Visual.transform.localScale += deltaScale;

            // Clamp the scale to prevent it from going below a certain threshold (e.g., 0.01) to avoid negative scaling.
            Visual.transform.localScale = new Vector3(
                Mathf.Max(Visual.transform.localScale.x, 0.01f),
                Mathf.Max(Visual.transform.localScale.y, 0.01f),
                Mathf.Max(Visual.transform.localScale.z, 0.01f)
            );
        }

        public void Delete()
        {
            OnBeingDeleted?.Invoke(gameObject);
            Destroy(Visual);
            // Destroy(SelectedVisual);
            Destroy(TransformableObjectReference);
            Destroy(gameObject);
        }


        public bool GetSelected()
        {
            return IsSelected;
        }

        //Get's called externally by the select logic
        public bool SetSelected(bool isSelected)
        {
            if(!canObjectBeSelected) return false;
            
            CustomLog.Instance.InfoLog("Got Selected " + gameObject.name);
            if (isSelected)
            {
                OnWasSelected?.Invoke(true);
            }
            else
            {
                OnWasSelected?.Invoke(false);
            }

            return true;

        }
    }
}