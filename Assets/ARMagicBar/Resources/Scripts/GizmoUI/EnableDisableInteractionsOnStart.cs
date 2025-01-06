using System;
using ARMagicBar.Resources.Scripts.TransformLogic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ARMagicBar.Resources.Scripts.GizmoUI
{
    
    /// <summary>
    /// Allows to disable certain interaction methods at the beginning. If you want to
    /// change the specific interaction methods at runtime, you can call the methods below on the GizmoHolderUI in any
    /// external script. 
    /// </summary>
    public class EnableDisableInteractionsOnStart : MonoBehaviour
    {
        [SerializeField] public bool IsSelectable = true;
        [Header("If you uncheck Transform, the rest is disabled by default.")]
        [SerializeField] public bool enableTransform = true;
        [SerializeField] public bool enableMove = true;
        [SerializeField] public bool enableRotate = true;
        [SerializeField] public bool enableScale = true;
        [SerializeField] public bool enableDelete = true;
        [SerializeField] public bool enableReset = true;
        
        [HideInInspector]
        [SerializeField] private GizmoHolderUI _gizmoHolderUI;

        [HideInInspector]
        public EnableDisableInteractionsOnStart Instance;

        [SerializeField] private TransformableObject transformableObject;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this; 
        }
        
        
        //The methods below can also be called at runtime by a custom script
        public void HideMoveGizmo()
        {
            _gizmoHolderUI.HideMoveGizmo();
        }

        public void ShowMoveGizmo()
        {
            _gizmoHolderUI.ShowMoveGizmo();
        }

        public void HideRotateGizmo()
        {
            _gizmoHolderUI.HideRotateGizmo();
        }

        public void ShowRotateGizmo()
        {
            _gizmoHolderUI.ShowRotateGizmo();
        }

        public void ShowScale()
        {
            _gizmoHolderUI.ShowScaleGizmo();
        }

        public void HideScale()
        {
            _gizmoHolderUI.HideScaleGizmo();
        }

        public void ShowReset()
        {
            _gizmoHolderUI.ShowResetTransformGizmo();
        }

        public void HideReset()
        {
            _gizmoHolderUI.HideResetTransformGizmo();
        }

        public void HideDelete()
        {
            _gizmoHolderUI.HideDeleteUIObject();
        }

        public void ShowDelete()
        {
            _gizmoHolderUI.ShowDeleteUIObject();
        }

        public void ShowAllTransform()
        {
            _gizmoHolderUI.ShowTransformElements();
        }

        public void HideTransformButton()
        {
            _gizmoHolderUI.EnableTransform = false;
            _gizmoHolderUI.HideShowTransformElementsButton();
        }

        public void EnableSelectable()
        {
            transformableObject.CanObjectBeSelected = true;
        }

        public void DisableSelectable()
        {
            transformableObject.CanObjectBeSelected = false;
        }

        // Start is called before the first frame update
        void Start()
        {
            if (IsSelectable == false)
            {
              DisableSelectable();  
            }
            
            
            if (enableMove == false)
            {
                HideMoveGizmo();
            }

            if (enableRotate == false)
            {
                HideRotateGizmo();
            }

            if (enableScale == false)
            {
                HideScale();
            }
            if (enableReset == false)
            {
                HideReset();
            }

            if (enableDelete == false)
            {
                HideDelete();
            }

            if (enableTransform == false)
            {
                HideTransformButton();
            }
        }
    }
}
