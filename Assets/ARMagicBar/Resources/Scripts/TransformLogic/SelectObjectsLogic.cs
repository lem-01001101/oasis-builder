using System;
using System.Collections.Generic;
using ARMagicBar.Resources.Scripts.Debugging;
using ARMagicBar.Resources.Scripts.Gizmo;
using ARMagicBar.Resources.Scripts.PlacementBar;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ARMagicBar.Resources.Scripts.TransformLogic
{
    /// <summary>
    /// Handles Raycasting and setting transformable Object to selected
    /// </summary>
    public class SelectObjectsLogic : MonoBehaviour
    {
        // private GameObject selectedObject;
        private TransformableObject selectedObject;
        
        private Camera mainCam;

        public static SelectObjectsLogic Instance;
        
        //Fires when object is deselected
        public event Action OnDeselectAll;
        
        //Fires when object is selected
        public event Action OnSelectObject;

        //Fires when Object is selected, contains info
        public event Action<TransformableObject> OnSelectObjectInfo;
        
        //Fires when a gizmo (XYZ) is selected
        public event Action<GameObject> OnGizmoSelected;

        // public event Action<Vector3> OnGizmoMoved;  

        private bool isManipulating;
        private bool isDragging; 
        
        private Vector3 initialPosition;
        private Vector3 axis;

        private bool disableTransformOptions;

        public bool DisableTransformOptions
        {
            get => disableTransformOptions;
            set => disableTransformOptions = value;
        }
        
        private void Awake()
        {
            if(Instance == null)
                Instance = this;
            
            
            mainCam = FindObjectOfType<Camera>();
        }

        public void DeselectAllObjects()
        {
            OnDeselectAll?.Invoke();
        }

        public TransformableObject GetSelectedObject()
        {
            return selectedObject;
        }
        
        public void DeleteSelectedObject()
        {
            selectedObject.Delete();
        }
        
        void Update()
        {
            if(FindObjectOfType<EventSystem>() ==false) return;
            
            //If any object from the bar is selected 
            if (PlacementBarLogic.Instance.GetPlacementObject() != null) return;

            //If the player is currently manipulating a placed objects
            if (GlobalSelectState.Instance.GetTransformstate() ==
                SelectState.manipulating) return;
            
            if(disableTransformOptions) return; 
            
#if UNITY_EDITOR
            //If not manipulating objects transform
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    CustomLog.Instance.InfoLog("UI Hit was recognized");
                    return;
                }

                TouchToRayCasting(Input.mousePosition);
            }

#endif
#if UNITY_IOS || UNITY_ANDROID
            if (Input.touchCount > 0 && Input.touchCount < 2 &&
                Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Touch touch = Input.GetTouch(0);
                
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = touch.position;

                List<RaycastResult> results = new List<RaycastResult>();

                EventSystem.current.RaycastAll(pointerData, results);

                if (results.Count > 0) {
                    // We hit a UI element
                    Debug.Log("We hit an UI Element");
                    return;
                }
                TouchToRayCasting(touch.position);
            }
#endif
        }

        //Shoot ray from the touch position 
        void TouchToRayCasting(Vector3 touch)
        {
            Ray ray = mainCam.ScreenPointToRay(touch);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                SelectObject(hit.collider.gameObject);
            }
            //Else, deselect all objects
            else 
            {
                OnDeselectAll?.Invoke();
                selectedObject = null;
            }
        }

        void SelectObject(GameObject objectThatWasHit)
        {
            //Select the specific Axis to move 
            if (isManipulating)
            {
                GizmoObject gizmoObject;
                if (objectThatWasHit.TryGetComponent(out gizmoObject))
                {
                    CustomLog.Instance.InfoLog("Gizmo was selected");
                    OnGizmoSelected?.Invoke(objectThatWasHit);
                }
                return;
            }
            
            CustomLog.Instance.InfoLog("SelectObject, Object that was hit" + 
                      objectThatWasHit.name);
            
            //Only one objects should be selected at a time
            
            TransformableObject obj;
            if (objectThatWasHit.GetComponentInParent<TransformableObject>())
            {
                
                //If transformable object -> Should be selected = false, => Return
                
                OnDeselectAll?.Invoke();
                selectedObject = null;
                
                obj = objectThatWasHit.GetComponentInParent<TransformableObject>();
                if(obj.CanObjectBeSelected == false) return;


                if (obj.CanObjectBeSelected == false)
                {
                    CustomLog.Instance.InfoLog("Object can't be selected.");
                    return;
                }
                
                selectedObject = obj;
                if (obj.GetSelected())
                {
                    obj.SetSelected(false);
                    return;
                }
                
                obj.SetSelected(true);
                OnSelectObject?.Invoke();
                OnSelectObjectInfo?.Invoke(obj);
            }
            else
            {
                OnDeselectAll?.Invoke();
                selectedObject = null;
            }
        }
    }
}