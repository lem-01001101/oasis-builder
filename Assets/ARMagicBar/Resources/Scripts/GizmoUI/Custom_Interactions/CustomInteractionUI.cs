using System;
using ARMagicBar.Resources.Scripts.PlacementObjects;
using UnityEngine;
using UnityEngine.UI;

namespace ARMagicBar.Resources.Scripts.GizmoUI.Custom_Interactions
{
    public class CustomInteractionUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Button customInteractionButton;
    
        private string nameOfCustomInteraction;
        private GameObject referenceToCustomObject;
        private ReferenceToSO referenceSOToCustomObject;
        private CustomInteractionDataSO customInteractionDataSo;
    

        public static event Action<(string nameOfInteraction, GameObject referenceToObject, ReferenceToSO referenceToSo, CustomInteractionDataSO customInteractionDataSo)> OnCustomInteractionTriggered;

        public void SetAttributes(string name, GameObject referenceToObject, ReferenceToSO refToSo, CustomInteractionDataSO interactionData)
        {
            nameOfCustomInteraction = name;
            referenceToCustomObject = referenceToObject;
            referenceSOToCustomObject = refToSo;
            customInteractionDataSo = interactionData;

        }
    
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetImage(Texture2D img)
        {
            
            Sprite newSprite = Sprite.
                Create(img, new Rect(0, 0, img.width, img.height), new Vector2(0.5f, 0.5f));

            icon.sprite = newSprite;
        }

    
        //When a custom Interaction button is triggered, function fires with different information and references
        void OnCustomInteractionsTriggered()
        {
            OnCustomInteractionTriggered?.Invoke((nameOfCustomInteraction, referenceToCustomObject, referenceSOToCustomObject, customInteractionDataSo));
        }
    
        private void Start()
        {
            customInteractionButton.onClick.AddListener(OnCustomInteractionsTriggered);
        }
    
    
    
    }
}
