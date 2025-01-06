using System;
using ARMagicBar.Resources.Scripts.Debugging;
using ARMagicBar.Resources.Scripts.TransformLogic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ARMagicBar.Resources.Scripts.PlacementBar
{
    [CreateAssetMenu]
    public class PlacementObjectSO : ScriptableObject
    {
        [SerializeField] public string nameOfObject; 
        [SerializeField] public Texture2D uiSprite;
        [SerializeField] public TransformableObject placementObject;
        
        [Header("Will be fully hidden on false")]
        [SerializeField] bool enableInUI = true;
        
        [Header("Will be greyed out on false")]
        [SerializeField] bool enableToSelect = true;

        [Header("Will show a number on the UI object representing the inventory amount")] 
        [SerializeField] bool enableAmountInInventory = false;
        [SerializeField] int amountInInventory;
        
        [Header("Will not spawn on false (useful for firing or spellcasting)")]
        [SerializeField] public bool IsPlaceable = true;
        
        public static event Action OnEnableItemInUITriggered;
        public static event Action OnEnableSelectItemTriggered;

        public static event Action OnInventoryAmountChanged;

        //Will only work in the beginning (before starting) by default
        public void SetEnableAmountInInventory(bool value)
        {
            enableAmountInInventory = value;
        }
        
        public bool GetEnableAmountInInventory()
        {
            return enableAmountInInventory;
        }

        //Grey out ui items 
        public void SetItemEnableToSelectInUI(bool isEnabled)
        {
            this.enableToSelect = isEnabled;
            OnEnableSelectItemTriggered?.Invoke();
        }

        public bool GetIsEnabledToSelectInUI()
        {
            return enableToSelect;
        }

        //Change inventory amount
        public void SetAmountInInventory(int amount)
        {
            amountInInventory = amount;
            OnInventoryAmountChanged?.Invoke();
        }

        
        public int GetAmountInInventory()
        {
            return amountInInventory;
        }
        
        //Hide items (e.g. when inventory is empty)
        public void SetIsEnabledInUI(bool showInUI)
        {
            CustomLog.Instance.InfoLog("Should enable item: " + this.name);
            this.enableInUI = showInUI;
            OnEnableItemInUITriggered?.Invoke();
        }

        public bool GetIsEnabledInUI()
        {
            return enableInUI;
        }
        
    }
}