using System;
using ARMagicBar.Resources.Scripts.Debugging;
using ARMagicBar.Resources.Scripts.PlacementBar;
using ARMagicBar.Resources.Scripts.TransformLogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARMagicBar.Resources.Scripts.PlacementBarUI
{
    public class PlacementObjectUiItem : MonoBehaviour
    {
        [SerializeField] private GameObject SelectedState;
        [SerializeField] private GameObject DisabledState; 
        [SerializeField] private RawImage normalIMG;
        [SerializeField] private TMP_Text amountIndicator;
        
        private TransformableObject correspondingObject;
        private PlacementObjectSO correspondingPlacementObjectSO;
        private Transform parentReference = null;

        private Vector2 initialPosition;
        private Vector2 newPosition; 

        private void Awake()
        {
            HideSelectedState();
            HideDisabledState();
            parentReference = transform.parent;
        }
        
        public void MoveItem(Vector2 inputPosition)
        {
            transform.position = Vector2.Lerp(transform.position, inputPosition, Time.deltaTime * 5);
            CustomLog.Instance.InfoLog("Moving Item to pos: " + inputPosition);
        }
        

        public void SetParentToNull()
        {
            transform.SetParent(null);
        }

        public void ResetParent()
        {
            transform.SetParent(parentReference);
        }

        public void ChangeParent(Transform newParent)
        {
            transform.SetParent(newParent);
        }

        public void PlaceItem()
        {
            
        }

        public bool IsDisabled()
        {
            return DisabledState.activeSelf;
        }

        public void SetCorrespondingObject(TransformableObject placementObject)
        {
            correspondingObject = placementObject; 
        }

        public PlacementObjectSO CorrespondingPlacementObjectSO
        {
            get => correspondingPlacementObjectSO;
            set => correspondingPlacementObjectSO = value;
        }
        
        public TransformableObject GetCorrespondingObject()
        {
            return correspondingObject;
        }

        public void SetAmountOfInventory(int amount)
        {
            amountIndicator.text = amount.ToString();
        }

        public void EnableDisableAmountIndicatorText(bool enable)
        {
            CustomLog.Instance.InfoLog("Should disable Indicator text.");
            amountIndicator.gameObject.SetActive(enable);
        }

        public void SetTexture(Texture2D tex2d)
        {
            normalIMG.texture = tex2d;
        }
        public void ShowSelectedState()
        {
            SelectedState.SetActive(true);
        }

        public void HideSelectedState()
        {
            SelectedState.SetActive(false);
        }

        public void HideDisabledState()
        {
            DisabledState.SetActive(false);
        }

        public void ShowDisabledState()
        {
            CustomLog.Instance.InfoLog("Show disabled state " + gameObject.name);
            DisabledState.SetActive(true);
        }

        public bool IsActive()
        {
            return SelectedState.gameObject.activeSelf;
        }

    }
}