using System;
using System.Collections;
using System.Collections.Generic;
using ARMagicBar.Resources.Scripts.Debugging;
using ARMagicBar.Resources.Scripts.Gizmo;
using ARMagicBar.Resources.Scripts.TransformLogic;
using UnityEngine;


namespace ARMagicBar.Resources.Scripts.ExampleExtension
{
    /// <summary>
    /// This script can sit on any Transformable object and will make it kinematic as long as it's selected.
    /// Moreover it will reposition the gizmos when 
    /// </summary>
    public class HandleRigidBodyTransformableObject : MonoBehaviour
    {
        [SerializeField] private GameObject _gizmoParentObject;
        private TransformableObject self; 
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponentInChildren<Rigidbody>();
            
            if(!_rigidbody) return;
            
            self = GetComponent<TransformableObject>();
        }

        void Start()
        {
            if(!_rigidbody) return;
                
            SelectObjectsLogic.Instance.OnSelectObjectInfo += OnObjectWasSelected;
            SelectObjectsLogic.Instance.OnDeselectAll += OnObjectsWereDeselected;
        }

        private void OnDestroy()
        {
            if(!_rigidbody) return;

            SelectObjectsLogic.Instance.OnSelectObjectInfo -= OnObjectWasSelected;
            SelectObjectsLogic.Instance.OnDeselectAll -= OnObjectsWereDeselected;
        }
        private void OnObjectsWereDeselected()
        {
            SetGizmoToPosition();
            _rigidbody.isKinematic = false;
        }
        
        void SetGizmoToPosition()
        {
            _gizmoParentObject.transform.position = _rigidbody.position;
        }

        private void OnObjectWasSelected(TransformableObject obj)
        {
            CustomLog.Instance.InfoLog("OnObjectWasSelected: "+ obj);
            
            if(obj != self) return;

            SetGizmoToPosition();
            CustomLog.Instance.InfoLog("Disabling Gravity for " + obj);
            _rigidbody.isKinematic = true;
        }
    }
}


