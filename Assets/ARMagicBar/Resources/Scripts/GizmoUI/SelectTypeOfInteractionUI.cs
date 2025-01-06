using System;
using UnityEngine;
using UnityEngine.UI;

namespace ARMagicBar.Resources.Scripts.GizmoUI
{
    public class SelectTypeOfInteractionUI : MonoBehaviour
    {
        public event Action OnSelectTransformInteractionButtonClicked;
        public event Action OnSelectCustominteractionButtonClicked; 
        
        [SerializeField] private GameObject TransformInteractionObject;
        [SerializeField] private GameObject CustomInteractionObject;

        [SerializeField] private Button SelectTransformInteractionButton;
        [SerializeField] private Button SelectCustomInteractionButton;

        public static SelectTypeOfInteractionUI Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);

        }

        private void Start()
        {
            SelectTransformInteractionButton.onClick.AddListener(() =>
            {
                OnSelectTransformInteractionButtonClicked?.Invoke();
            });
            
            SelectCustomInteractionButton.onClick.AddListener(() => 
                OnSelectCustominteractionButtonClicked?.Invoke());
        }

    }
}