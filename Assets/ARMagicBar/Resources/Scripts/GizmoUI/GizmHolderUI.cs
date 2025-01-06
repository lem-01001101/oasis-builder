using System;
using System.Collections.Generic;
using ARMagicBar.Resources.Scripts.Debugging;
using ARMagicBar.Resources.Scripts.GizmoUI.Custom_Interactions;
using ARMagicBar.Resources.Scripts.Other;
using ARMagicBar.Resources.Scripts.TransformLogic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ARMagicBar.Resources.Scripts.GizmoUI
{
    public class GizmoHolderUI : MonoBehaviour
    {
        [SerializeField] public GameObject selectInteractionTypeCanvas;
        [SerializeField] public GameObject customInteractionHolderCanvas; 
        [SerializeField] public GameObject  gizmoHolderCanvas;

        private bool hasCustomInteractions = false;

        public bool HasCustomInteraction
        {
            set => hasCustomInteractions = value;
            get => hasCustomInteractions;
        }

        private bool enableTransform = true;

        public bool EnableTransform
        {
            set => enableTransform = value;
            get => enableTransform;
        }
        
        [SerializeField] public MoveGizmoUI moveGizmoUI;
        [SerializeField] public RotateGizmoUI rotateGizmoUI;
        [SerializeField] public ScaleGizmoUI scaleGizmoUI;
        [SerializeField] public BackToGizmoUI backToGizmoUI;
        [SerializeField] public DeleteObjectGizmoUI deleteObjectGizmoUI;
        [SerializeField] public ResetTransformGizmoUI resetTransformGizmoUI;
        [SerializeField] public CustomInteractionsHolderUI customInteractionHolderUI;
        [SerializeField] public SelectTransformGizmoUI selectTransformGizmoUI;
        [SerializeField] public SelectTypeOfInteractionUI selectTypeOfInteractionUI;

        [SerializeField] private CustomInteractionManager customInteractionsManager;
        [SerializeField] private EnableDisableInteractionsOnStart enableDisableInteractionsOnStart;
        
        public List<IGizmos> iGizmos; 
        
        //Reference to the buttons
        [SerializeField] public Button moveButtonReference;
        [SerializeField] public Button rotateButtonReference;
        [SerializeField] public Button scaleButtonReference;
        [SerializeField] public Button deleteButtonReference;
        [SerializeField] public Button backButtonReference;
        [SerializeField] public Button resetTransformButtonReference;

        public event Action moveButtonToggled;
        public event Action rotateButtonToggled;  
        public event Action scaleButtonToggled;
        public event Action deleteButtonToggled;

        public event Action resetTransformButtonToggled; 

        public static event Action OnAnyGizmoUIButtonToggled;
        public static event Action OnBackToUIGizmosToggled; 
        

        private TransformableObject _transformableObject;
        private Camera mainCamera;


        public static GizmoHolderUI Instance;  
        
        void AddGizmos()
        {
            iGizmos = new List<IGizmos>() { moveGizmoUI, rotateGizmoUI, scaleGizmoUI, resetTransformGizmoUI };
        }


        void SetCanvasCamera()
        {
            Canvas objectCanvas = GetComponent<Canvas>();
            objectCanvas.worldCamera = mainCamera;
        }

        private void Awake()
        {
            Instance = this;
            _transformableObject = GetComponentInParent<TransformableObject>();
            

            if (customInteractionHolderCanvas != null && customInteractionsManager.GetAmountOfCustomInteractions() > 0)
            {
                hasCustomInteractions = true;
            }            
            
            if (mainCamera == default)
            {
                mainCamera = FindObjectOfType<Camera>();
                SetCanvasCamera();
            }
        }

        private void OnDestroy()
        {
            _transformableObject.OnWasSelected -= OnTransformableObjectWasSelected; 
            SelectObjectsLogic.Instance.OnDeselectAll -= TransformableObjectOnDeselectAll;
        }

        private void Start()
        {
            AddGizmos();
            _transformableObject.OnWasSelected += OnTransformableObjectWasSelected; 
            SelectObjectsLogic.Instance.OnDeselectAll += TransformableObjectOnDeselectAll;
            
            
            resetTransformButtonReference.onClick.AddListener(() =>
            {
                CustomLog.Instance.InfoLog("ResetButtonHit");
                resetTransformButtonToggled?.Invoke();
            });
            
            backButtonReference.onClick.AddListener(() =>
            {
                CustomLog.Instance.InfoLog("Back Button Hit");
                ShowTransformElements();
                HideBackToGizmoUI();
                OnBackToUIGizmosToggled?.Invoke();
            });
            
            moveButtonReference.onClick.AddListener(() =>
            {
                CustomLog.Instance.InfoLog("Move UI Button Clicked");
                moveButtonToggled?.Invoke();
                OnAnyGizmoUIButtonToggled?.Invoke();
                
                HideTransformElements();
                ShowBackToGizmoUI();
                // ShowSingleGizmo(moveGizmoUI);
            });
            rotateButtonReference.onClick.AddListener(() =>
            {
                CustomLog.Instance.InfoLog("Rotate UI Button clicked");
                rotateButtonToggled?.Invoke();
                OnAnyGizmoUIButtonToggled?.Invoke();
                
                HideTransformElements();
                ShowBackToGizmoUI();
                // ShowSingleGizmo(rotateGizmoUI);

            });
            scaleButtonReference.onClick.AddListener(() =>
            {
                CustomLog.Instance.InfoLog("Scale UI Button clicked");
                scaleButtonToggled?.Invoke();
                OnAnyGizmoUIButtonToggled?.Invoke();
                
                HideTransformElements();
                ShowBackToGizmoUI();

                // ShowSingleGizmo(scaleGizmoUI);

            });
            deleteButtonReference.onClick.AddListener(() =>
            {
                deleteButtonToggled?.Invoke();
            });
            
            selectTypeOfInteractionUI.OnSelectTransformInteractionButtonClicked += SelectTypeOfInteractionUIOnOnSelectTransformInteractionButtonClicked;
            selectTypeOfInteractionUI.OnSelectCustominteractionButtonClicked += SelectTypeOfInteractionUIOnOnSelectCustominteractionButtonClicked;
            
            mainCamera = FindObjectOfType<Camera>();


            HideBackToGizmoUI();
            HideTransformElements();
            HideSelectTypeOfInteractionGUI();
            HideCustomInteractions();

        }
        

        private void SelectTypeOfInteractionUIOnOnSelectCustominteractionButtonClicked()
        {
            HideSelectTypeOfInteractionGUI();
            ShowCustomInteractionHolder();
        }

        private void SelectTypeOfInteractionUIOnOnSelectTransformInteractionButtonClicked()
        {
            HideSelectTypeOfInteractionGUI();
            ShowTransformInteractions();
        }

        public void HideMoveGizmo()
        {
            moveGizmoUI.Hide();
        }

        public void ShowMoveGizmo()
        {
            moveGizmoUI.Show();
        }

        public void HideRotateGizmo()
        {
            rotateGizmoUI.Hide();
        }

        public void ShowRotateGizmo()
        {
            rotateGizmoUI.Show();
        }

        public void HideScaleGizmo()
        {
            scaleGizmoUI.Hide();
        }

        public void ShowScaleGizmo()
        {
            scaleGizmoUI.Show();
        }

        public void HideResetTransformGizmo()
        {
            resetTransformGizmoUI.Hide();
        }

        public void ShowResetTransformGizmo()
        {
            resetTransformGizmoUI.Show();
        }

        public void HideDeleteUIObject()
        {
            if (deleteObjectGizmoUI)
            {
                deleteObjectGizmoUI.Hide();
            }
            else
            {
                Debug.LogWarning(AssetName.NAME + " delete object Gizmo is null, you maybe use this method on a 0.78 version asset. Please recreate this Asset with Version 1.0 or later.");
            }
        }

        public void ShowDeleteUIObject()
        {
            if (deleteObjectGizmoUI)
            {
                deleteObjectGizmoUI.Show();
            }
            else
            {
                Debug.LogWarning(AssetName.NAME + " delete object Gizmo is null, you maybe use this method on a 0.78 version asset. Please recreate this Asset with Version 1.0 or later.");
            }
        }
        
        
        private void Update()
        {
            if (isActiveAndEnabled && mainCamera != null)
            {
                transform.LookAt(mainCamera.transform);
            }
        }
        
        private void TransformableObjectOnDeselectAll()
        {
            CustomLog.Instance.InfoLog("Deselect All!");
            HideTransformElements();
            HideSelectTypeOfInteractionGUI();
            HideCustomInteractions();
        }

        private void OnTransformableObjectWasSelected(bool obj)
        {
            if (!hasCustomInteractions && enableTransform)
            {
                ShowTransformElements();
                return;
            }  
            
            if (!enableTransform &&  hasCustomInteractions)
            {
                CustomLog.Instance.InfoLog("GizmoHolderUI hasCustom Interaction but no Transform");
                ShowCustomInteractionHolder();
                return;
            }

            ShowSelectInteractionGUI();

        }


        void ShowSelectInteractionGUI()
        {
            selectTypeOfInteractionUI.Show();
        }

        void HideSelectTypeOfInteractionGUI()
        {
            selectTypeOfInteractionUI.Hide();
        }
        
        void ShowTransformInteractions()
        {
            ShowTransformElements();
        }

        void ShowCustomInteractionHolder()
        {
            customInteractionHolderUI.Show();
        }

        void ShowCustomInteractions()
        {
            
        }

        void HideCustomInteractions()
        {
            customInteractionHolderUI.Hide();
        }


        void ShowBackToGizmoUI()
        {
            backToGizmoUI.gameObject.SetActive(true);
        }
        
        void HideBackToGizmoUI()
        {
            backToGizmoUI.gameObject.SetActive(false);

        }
        
        public void ShowTransformElements()
        {
           gizmoHolderCanvas.SetActive(true); 
        }

        public void HideTransformElements()
        {
            gizmoHolderCanvas.SetActive(false); 
        }

        public void HideShowTransformElementsButton()
        {
            selectTransformGizmoUI.gameObject.SetActive(false);
        }

        public void ShowShowTransformElementsButton()
        {
            selectTransformGizmoUI.gameObject.SetActive(true);
        }
        
    }
}