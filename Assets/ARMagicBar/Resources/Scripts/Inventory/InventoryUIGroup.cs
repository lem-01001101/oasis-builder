using System;
using System.Collections.Generic;
using System.Linq;
using ARMagicBar.Resources.Scripts.Debugging;
using ARMagicBar.Resources.Scripts.PlacementBar;
using ARMagicBar.Resources.Scripts.PlacementBarUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARMagicBar.Resources.Scripts.Inventory
{
    public class InventoryUIGroup : MonoBehaviour
    {
        [SerializeField] private PlaceableObjectSODatabase databaseIdentifier; 
        [SerializeField] private TMP_Text _tmpText;
        [SerializeField] private Transform parent;
        [SerializeField] private Transform objectsParent;
        [SerializeField] private RectTransform groupHolder;
        [SerializeField] private GridLayoutGroup UiElementHolder;
        
        private ScreenOrientation lastOrientation;

        
        private float rectHeight;
        private float rectWidth;
        private int amountOfItemsPerRow;
        private int amountOfRows;
        private float heightOfParent;

        private void Awake()
        {
            lastOrientation = Screen.orientation;
            CustomLog.Instance.InfoLog("Orientation: " + Screen.orientation);

            
            PlacementBarUIElements.OnAllUiElementsAdded += OnAllUIElementsAdded;
            PlacementBarUIElements.OnUiElementsChanged += AdjustRowSizeOnUIItemsChanged;
            PlacementObjectSO.OnEnableItemInUITriggered += AdjustRowSizeOnUIItemsChanged;
        }

        private void Update()
        {
            // Use screen dimensions to detect orientation
            if (Screen.width > Screen.height && lastOrientation != ScreenOrientation.LandscapeLeft)
            {
                lastOrientation = ScreenOrientation.LandscapeLeft;
                OnOrientationChanged();
            }
            else if (Screen.width < Screen.height && lastOrientation != ScreenOrientation.Portrait)
            {
                lastOrientation = ScreenOrientation.Portrait;
                OnOrientationChanged();
            }   
        }
        
        void OnOrientationChanged()
        {
            CustomLog.Instance.InfoLog("Screen Orientation changed");
            AdjustRowSizeOnUIItemsChanged();
        }

        private void AdjustRowSizeOnUIItemsChanged()
        {
            CustomLog.Instance.InfoLog("AdjustRowSizeOnUIItemsChanged");
            SetAmountOfItemPerRow(rectWidth, out amountOfItemsPerRow);
            CalculateRowAmount(out amountOfRows, amountOfItemsPerRow); 
            SetHeightOfParent(out heightOfParent, amountOfRows, rectHeight);
            ApplyHeightToParentObject(heightOfParent);
        }
        
        private void OnAllUIElementsAdded(PlacementObjectUiItem obj)
        {
            SetRectHeightAndWidth(obj, out rectWidth, out rectHeight);
            SetAmountOfItemPerRow(rectWidth, out amountOfItemsPerRow);
            CalculateRowAmount(out amountOfRows, amountOfItemsPerRow); 
            SetHeightOfParent(out heightOfParent, amountOfRows, rectHeight);
            ApplyHeightToParentObject(heightOfParent);
        }

        void SetAmountOfItemPerRow(float rectWidth, out int amountOfItemsPerRow)
        {
            amountOfItemsPerRow = Mathf.FloorToInt(Screen.width / (rectWidth * 1.2f));
            UiElementHolder.constraintCount = amountOfItemsPerRow;
        }

        private void SetRectHeightAndWidth(PlacementObjectUiItem obj, out float rectWidth, out float rectHeight)
        {
            rectHeight = obj.GetComponent<RectTransform>().rect.height;
            rectWidth = obj.GetComponent<RectTransform>().rect.width;
        }
        

        void CalculateRowAmount(out int rowAmount, int itemsPerRow)
        {
            float RowDividedByAmount = (float) databaseIdentifier.PlacementObjectSos.Where(x => x.placementObject.gameObject.activeSelf).ToList().Count / itemsPerRow;
            CustomLog.Instance.InfoLog("RowDividedByAmount: " +RowDividedByAmount); 
            
            rowAmount = Mathf.CeilToInt(RowDividedByAmount);
        }


        void SetHeightOfParent(out float HeightOfParent, int amountOfRows, float rectHeight)
        {
            HeightOfParent = Mathf.Max(amountOfRows * rectHeight, rectHeight) + 100f;
        }

        void ApplyHeightToParentObject(float parentHeight)
        {
            // Adjust the height while keeping the current width, so all UI objects
            // are correctly covered 
            Vector2 size = groupHolder.sizeDelta;
            size.y = parentHeight;
            size.x = Screen.width;
            groupHolder.sizeDelta = size;
        }



        public int GetActiveChildCount()
        {
            int activeChilds = 0;

            PlacementObjectUiItem[] children = objectsParent.GetComponentsInChildren<PlacementObjectUiItem>();
        
            foreach (var child in children)
            {
                if (child.gameObject.activeSelf)
                {
                    activeChilds++;
                }
            }
            return activeChilds;
        }

        public bool ActivateDeactiveInventoryUIBasedOnChildren()
        {
            foreach (var placementObjectSo in databaseIdentifier.PlacementObjectSos)
            {
                if (placementObjectSo.GetIsEnabledInUI())
                {
                    return true;
                }
            }

            return false;
        }

        public List<PlacementObjectSO> ReturnActivePlacementObjectSos()
        {
            List<PlacementObjectSO> activePlacementObjects = new();
        
            foreach (var placementObjectSo in databaseIdentifier.PlacementObjectSos)
            {
                if (placementObjectSo.GetIsEnabledInUI())
                {
                    activePlacementObjects.Add(placementObjectSo);
                }
            }
            return activePlacementObjects;
        }

        public PlaceableObjectSODatabase DatabaseIdentifier
        {
            get => databaseIdentifier;
            set => databaseIdentifier = value;
        }

        public string NameOfInventoryGroup
        {
            get => _tmpText.text;
            set => _tmpText.text = value;
        }

        public Transform GetParent
        {
            get => parent;
        }
    

        public void SetIdentifier(PlaceableObjectSODatabase database)
        {
            databaseIdentifier = database;
        }


        public void SetText(string text)
        {
            _tmpText.text = text;
        }

        public void AddObjectToParent(Transform toAddObject)
        {
            toAddObject.parent = parent;
        }


    }
}
