using System;
using System.Collections.Generic;
using System.Linq;
using ARMagicBar.Resources.Scripts.Debugging;
using ARMagicBar.Resources.Scripts.Inventory;
using ARMagicBar.Resources.Scripts.PlacementBar;
using ARMagicBar.Resources.Scripts.TransformLogic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ARMagicBar.Resources.Scripts.PlacementBarUI
{
    public class PlacementBarUIElements : MonoBehaviour
    {
        // panel

        [HideInInspector]
        [SerializeField] private Transform uiObjectParent;
        [HideInInspector]
        [SerializeField] private RectTransform uiObjectParentRectTransform;

        [SerializeField] private GridLayoutGroup placementBarParentGridLayoutGroup;
        [HideInInspector]
        [SerializeField] private InventoryUIGroup inventoryUIGroup;
        [HideInInspector]
        [SerializeField] private Transform uiInventoryObject;
        [HideInInspector]
        [SerializeField] private Transform uiInventoryGroupsObjectParent;

        [SerializeField] private ScrollRect _scrollRect;

        //The UI Items are unique
        private HashSet<InventoryUIGroup> _inventoryUIGroups = new();
        
        [Header("Double-click to customize. Click once & duplicate to create variants.")] 
        [SerializeField] private PlacementObjectUiItem uiItemPrefab;
    
        [Header("Change the texture of the page, hide, inventory icon")]
        [SerializeField] private Texture2D hideTexture;
        [SerializeField] private Texture2D paginationTexture;
        [SerializeField] private Texture2D inventoryTexture;

        private PlacementObjectUiItem inventoryUiItem;
        private PlacementObjectUiItem hideUiItemObject;
        
        [SerializeField] private Transform dragCanvas; 
            
        private PlacementObjectUiItem _selectedUiItemElement; 
        
        private List<(int, PlacementObjectUiItem)> pageToUIObjects = new();

        List<PlacementObjectUiItem> _allItems = new();
        private List<PlacementObjectUiItem> _placementBarItems = new();
        private List<PlacementObjectUiItem> _inventoryItems = new();

        [Header("If true, it will deselect the object when it was placed.")] 
        [SerializeField] private bool autoDeselectOnPlace = false;

        [Header("Determine if there should be a hide icon")]
        public bool enableHide = true;
        public ManageObjectType setManageObjectType = ManageObjectType.inventory; 
        
        // public bool enablePage = true;
        // public bool enableInventory = true;

        [Header("If disabled, there will be no inventory / paginate option")]
        public bool enableInventoryOption = true;

        [HideInInspector]
        [Header("Determine the Position of the placement bar")]
        public PlacementBarPosition PlacementBarPosition = PlacementBarPosition.bottom;
    
        public event Action<TransformableObject> OnUiElementSelected;
    
        //For Debug purposes
        // public event Action<PlacementObjectSO> OnUiElementSelectedSO;
        public static event Action<PlacementObjectUiItem> OnAllUiElementsAdded;
        public static event Action OnUiElementsChanged; 

    
        public static PlacementBarUIElements Instance;
    
        private bool barIsHidden = false;
        private const string HIDEOBJECT_NAME = "Hide Obejct";

        private int currentPage = 1;
    
        [FormerlySerializedAs("MAX_ITEMS_PER_PAGE")] [Header("Determine the amount of items per page")]
        public int ItemsPerPage = 3; 
        private int maxPage = 1;
        private const string PAGINATIONOBJECT_NAME = "Pagination Object";
        
        private const string INVENTORY_OBJECT_NAME = "Inventory Object";
        private bool isInventoryOpen = false;
        private bool isDraggingItem = false;

        [SerializeField] private RawImage placementBarArea;
    
        // Start is called before the first frame update
        private void Awake()
        {
            if(Instance == null)
                Instance = this;

            SetPlacementBarScreenPosition(PlacementBarPosition);
        }

        private float PixelThreshold = 50f;
        
        bool IsPointerOverPlacementBar(Vector2 screenPosition)
        {
            CustomLog.Instance.InfoLog("ScreePos when checking over Placement bar " + screenPosition);
            
            //If a hit object in UI has a placement bar ui object item attached 

            List<RaycastResult> results = new();
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = screenPosition;
            
            EventSystem.current.RaycastAll(pointerEventData, results);

            foreach (var raycastResult in results)
            {
                CustomLog.Instance.InfoLog("Pointer Event data , raycast result: " + raycastResult.gameObject.name + "screenPOS => "+ screenPosition);
                if (raycastResult.gameObject.GetComponentInParent<PlacementBarUIParent>() || 
                    raycastResult.gameObject.GetComponent<PlacementBarUIParent>())
                {
                    CustomLog.Instance.InfoLog("Pointer Event data: Is current over PlacementBar => true");
                    return true; 
                }
            }

            return false;
        }
 
        public void SetPlacementBarScreenPosition(PlacementBarPosition position)
        {
            switch (position)
            {
                case PlacementBarPosition.bottom:
                    placementBarParentGridLayoutGroup.startAxis = GridLayoutGroup.Axis.Vertical;
                    uiObjectParentRectTransform.anchorMin = new Vector2(0.5f, 0); 
                    uiObjectParentRectTransform.anchorMax = new Vector2(0.5f, 0);
                    uiObjectParentRectTransform.pivot = new Vector2(0.5f, 0);
                    uiObjectParentRectTransform.anchoredPosition = new Vector2(0, 50);  
                    break;
                case PlacementBarPosition.left:
                    placementBarParentGridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
                    uiObjectParentRectTransform.anchorMin = new Vector2(0, 0.5f); 
                    uiObjectParentRectTransform.anchorMax = new Vector2(0, 0.5f);
                    uiObjectParentRectTransform.pivot = new Vector2(0, 0.5f);
                    uiObjectParentRectTransform.anchoredPosition = new Vector2(0, 0);  
                    break;
                case PlacementBarPosition.right:
                    placementBarParentGridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
                    uiObjectParentRectTransform.anchorMin = new Vector2(1, 0.5f); 
                    uiObjectParentRectTransform.anchorMax = new Vector2(1, 0.5f);
                    uiObjectParentRectTransform.pivot = new Vector2(1, 0.5f);
                    uiObjectParentRectTransform.anchoredPosition = new Vector2(0, 0);  
                    break;
                case PlacementBarPosition.top:
                    placementBarParentGridLayoutGroup.startAxis = GridLayoutGroup.Axis.Vertical;
                    uiObjectParentRectTransform.anchorMin = new Vector2(0.5f, 1); 
                    uiObjectParentRectTransform.anchorMax = new Vector2(0.5f, 1);
                    uiObjectParentRectTransform.pivot = new Vector2(0.5f, 1);
                    uiObjectParentRectTransform.anchoredPosition = new Vector2(0, 0);  
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }
        }

        public void DisableElement(int index)
        {
            CustomLog.Instance.InfoLog("Should disable Element at " + index);
            CustomLog.Instance.InfoLog("Amount of UIElements is" + _allItems.Count);
        
            if(_allItems[index])
                _allItems[index].ShowDisabledState();
        }
    
        public void EnableElement(int index)
        {
            if(_allItems[index])
                _allItems[index].HideDisabledState();
        }

        void OpenInventory()
        {
            isInventoryOpen = true;
            uiInventoryObject.gameObject.SetActive(true);
        }

        void CloseInventory()
        {
            isInventoryOpen = false;
            uiInventoryObject.gameObject.SetActive(false);
        }

        // ui panel toggle

        void Start()
        {
            _allItems = new List<PlacementObjectUiItem>();
            SetUIElements();
        
            ARPlacementPlaneMesh.Instance.OnSpawnObject += InstanceOnOnSpawnObject;
            PlacementObjectSO.OnEnableItemInUITriggered += ReloadUIElements;
            PlacementObjectSO.OnEnableSelectItemTriggered += ReloadUIElements;
            PlacementObjectSO.OnInventoryAmountChanged += ReloadUIElements;
        }

        private void OnDestroy()
        {
            ARPlacementPlaneMesh.Instance.OnSpawnObject -= InstanceOnOnSpawnObject;

        }

        private void InstanceOnOnSpawnObject(TransformableObject obj)
        {
            if (autoDeselectOnPlace)
            {
                DeselectOnPlacement();
            }
        }

        private void Update()
        {
            if(GlobalSelectState.Instance.GetTransformstate() != SelectState.unselected) return;
            
            CheckForSelectUIElement();
        }
        
        public void ReloadUIElements()
        {
            if (setManageObjectType == ManageObjectType.inventory)
            {
                RefreshEnableItemsInUI();
                RefreshEnableInventory();
            }
            
            
            RefreshInventoryAmount();
            RefreshEnabledSelectItems();

        }

        void RefreshInventoryAmount()
        {
            foreach (var item in _allItems)
            {
                if(item == null) continue;

                int amountInInventory = item.GetCorrespondingObject().GetCorrespondingPlacementObject
                    .GetAmountInInventory();

                item.SetAmountOfInventory(amountInInventory);

                if (amountInInventory == 0 && 
                    item.GetCorrespondingObject().GetCorrespondingPlacementObject.GetEnableAmountInInventory())
                {
                    DeselectAllElements();
                }
            }
            
        }

        void RefreshEnableInventory()
        {
            foreach (var inventoryUIGroup in _inventoryUIGroups)
            {
                if(!inventoryUIGroup) return;
                
                if (inventoryUIGroup.GetActiveChildCount() == 0)
                {
                    inventoryUIGroup.gameObject.SetActive(false);
                }
                else
                {
                    inventoryUIGroup.gameObject.SetActive(true);
                }
            }
        }

        void RefreshEnabledSelectItems()
        {
            foreach (var uiItem in _allItems)
            {
                if(uiItem == null) continue;

                CustomLog.Instance.InfoLog("RefreshEnabledItems: " + uiItem +  
                                           " GetShowInUI? " + uiItem.CorrespondingPlacementObjectSO.GetIsEnabledInUI());
                
                // if(!uiItem.gameObject.activeSelf) continue;
                
                if (!uiItem.CorrespondingPlacementObjectSO.GetIsEnabledToSelectInUI())
                {
                    CustomLog.Instance.InfoLog("Should disable Item for: " +uiItem.CorrespondingPlacementObjectSO.name);
                    uiItem.ShowDisabledState();
                }
                else
                {
                    uiItem.HideDisabledState();
                } 
            }
            
        }
        

        void RefreshEnableItemsInUI()
        {
            foreach (var uiItem in _allItems)
            {
                if(uiItem == null) continue;
                CustomLog.Instance.InfoLog("RefreshEnabledItems: " + uiItem +  
                                           " GetShowInUI? " + uiItem.CorrespondingPlacementObjectSO.GetIsEnabledInUI());
                
                if (!uiItem.CorrespondingPlacementObjectSO.GetIsEnabledInUI())
                {
                    uiItem.gameObject.SetActive(false);
                }
                else
                {
                    uiItem.gameObject.SetActive(true);
                } 
            }
        }


        //Create a UI Element for each Placement-object and add the UI texture onto it 
        void SetUIElements()
        {
            //Get all Elements (SO's) from the placementBarLogic 
            List<(PlacementObjectSO placementObj, PlaceableObjectSODatabase database)> allPlacement = PlacementBarLogic.Instance.GetAllObjects();
            
            
            if (allPlacement == null || allPlacement == default)
            {
                CloseInventory();
                CustomLog.Instance.InfoLog("No placeable objects.");
                return;
            }
            
            if (allPlacement.Count == 0)
            {
                EnsureListCapacity(allPlacement, 1);
            }
            
            //Paginate option
            if (setManageObjectType == ManageObjectType.paginate)
            {
                if (enableInventoryOption)
                {
                    PlacementObjectUiItem paginationUiItem = Instantiate(uiItemPrefab, parent: uiObjectParent);
                    paginationUiItem.AddComponent<PaginationUIElement>();
                    paginationUiItem.gameObject.name = PAGINATIONOBJECT_NAME;
                    paginationUiItem.SetTexture(paginationTexture);
                    paginationUiItem.EnableDisableAmountIndicatorText(false);
                }
                
                maxPage = (int) Math.Ceiling((double) allPlacement.Count / ItemsPerPage);

                List<PlacementObjectSO> placementObjectSos = allPlacement.Select(x => x.placementObj).ToList();
                SpawnItemsWithPaginateOption(placementObjectSos);
                ReloadUIElements();
                ShowItemsOnCurrentPage();
                CloseInventory();
            } 
            //Inventory option
            else 
            {
                if (enableInventoryOption)
                {
                    inventoryUiItem = Instantiate(uiItemPrefab, parent: uiObjectParent);
                    inventoryUiItem.AddComponent<InventoryUIElement>();
                    inventoryUiItem.gameObject.name = INVENTORY_OBJECT_NAME;
                    inventoryUiItem.SetTexture(inventoryTexture);
                    inventoryUiItem.EnableDisableAmountIndicatorText(false);
                }
                
                SpawnItemsWithInventoryOptions(allPlacement);
                ReloadUIElements();
                CloseInventory();
            }
            

            //Hide item
            if (enableHide)
            {
                hideUiItemObject = Instantiate(uiItemPrefab, parent: uiObjectParent);
                hideUiItemObject.AddComponent<HideUIElement>();
                hideUiItemObject.gameObject.name = HIDEOBJECT_NAME;
                hideUiItemObject.SetTexture(hideTexture);

                hideUiItemObject.EnableDisableAmountIndicatorText(false);
            }
            
            DebugPrintDictionary();
            CustomLog.Instance.InfoLog("Should Invoke OnAllUiElementsAdded");
            OnAllUiElementsAdded?.Invoke(uiItemPrefab);
        }
        
        void SpawnItemsWithInventoryOptions(List<(PlacementObjectSO objectSO, PlaceableObjectSODatabase databaseSO)> placementObjectSo)
        {
            foreach (var item in placementObjectSo)
            {
                if(item == default) continue;
                //Inventory group equals the PlaceableObjectSODatabase that an object is coming from. 
                InventoryUIGroup currenInventoryGroup = (_inventoryUIGroups.FirstOrDefault(x => 
                    x.DatabaseIdentifier == item.databaseSO));

                if (currenInventoryGroup == default)
                {
                    currenInventoryGroup = CreateNewInventoryGroup(item.databaseSO);
                }
                
                if (placementObjectSo.IndexOf(item) < ItemsPerPage)
                {
                    PlacementObjectUiItem uiItem = SpawnItemsWithParent(item.objectSO, uiObjectParent);
                    _allItems.Add(uiItem);
                }
                else
                {
                    PlacementObjectUiItem uiItem = SpawnItemsWithParent(item.objectSO, currenInventoryGroup.GetParent);
                    // _inventoryItems.Add(uiItem);
                    _allItems.Add(uiItem);
                }
            }
        }


        InventoryUIGroup CreateNewInventoryGroup(PlaceableObjectSODatabase referenceToDatabase)
        {
            CustomLog.Instance.InfoLog($"Inventory UI Group prefab => {inventoryUIGroup.name}");
            
            InventoryUIGroup group = Instantiate(inventoryUIGroup, parent: uiInventoryGroupsObjectParent);
            
            CustomLog.Instance.InfoLog($"Should Create new Inventory group {group.name} for " +
                                       $"database {referenceToDatabase} withe name : {referenceToDatabase.DatabaseName}");
            group.NameOfInventoryGroup = referenceToDatabase.DatabaseName;
            group.DatabaseIdentifier = referenceToDatabase;
            _inventoryUIGroups.Add(group);
            return group;
        }
        void SetCorrectPositionForInventoryAndHideButton()
        {
            if (inventoryUiItem)
            {
                CustomLog.Instance.InfoLog("Setting correct position for inventory and hide buttons.");
                inventoryUiItem.transform.SetSiblingIndex(0);
            }

            if (hideUiItemObject)
            {
                int childCount = placementBarParentGridLayoutGroup.transform.childCount;
                hideUiItemObject.transform.SetSiblingIndex(childCount);
            }
        }


        void DebugLogInventoryUIGroups()
        {
            CustomLog.Instance.InfoLog("DebugLogInventoryUIGroups");
            foreach (var inventoryUIGroup in _inventoryUIGroups)
            {
                CustomLog.Instance.InfoLog("inventoryUIGroup: " + inventoryUIGroup.NameOfInventoryGroup);
                CustomLog.Instance.InfoLog("database identifier: " + inventoryUIGroup.DatabaseIdentifier.DatabaseName);
            }
        }
        
        void AdjustUIBarInventoryItems()
        {
            foreach (var item in _allItems)
            {
                if(item == null) continue;
                // if(_allItems.IndexOf(item) == _allItems.Count-1 || _allItems.IndexOf(item) == 0) continue;
                CustomLog.Instance.InfoLog("AdjustUIBarInventoryItems, item: " + item + " Name: "+ item.name);

                
                if (_allItems.IndexOf(item) < ItemsPerPage)
                {
                    // item.ChangeParent(uiObjectParent);
                }
                else
                {
                    DebugLogInventoryUIGroups();
                    
                    
                    InventoryUIGroup uiGroup = _inventoryUIGroups.FirstOrDefault(x =>
                        x.DatabaseIdentifier.PlacementObjectSos.Contains(item.CorrespondingPlacementObjectSO));
                    
                    var allObjects = PlacementBarLogic.Instance.GetAllObjects();

                    if (uiGroup == default)
                    {
                        uiGroup = CreateNewInventoryGroup(allObjects.First(x => x.Item1 == item).Item2);
                    }
                    
                    Transform parent = uiGroup.GetParent;
                    
                    item.ChangeParent(parent);
                }
            }

            SetCorrectPositionForInventoryAndHideButton();
        }
        
        
        void SpawnItemsWithPaginateOption(List<PlacementObjectSO> placementObjectSo)
        {
            foreach (var placementObject in placementObjectSo)
            {
                PlacementObjectUiItem placementObjectUiItem = SpawnItemsWithParent(placementObject, uiObjectParent);
                (int, PlacementObjectUiItem) indexToObject = new(CalculatePage(placementObjectSo.IndexOf(placementObject) + 1),
                    placementObjectUiItem);
           
                pageToUIObjects.Add(indexToObject);
                _allItems.Add(placementObjectUiItem);
            }
        }

        PlacementObjectUiItem SpawnItemsWithParent(PlacementObjectSO placementObjectSo, Transform parent)
        {
            PlacementObjectUiItem instantiatedUiItemElement = Instantiate(uiItemPrefab, parent: parent);
            CustomLog.Instance.InfoLog($"Creating PlacementObjectUiItem, instantiatedUiItemElement: {uiItemPrefab.name}");

            
            instantiatedUiItemElement.SetTexture(placementObjectSo.uiSprite); 
            
            instantiatedUiItemElement.SetCorrespondingObject(placementObjectSo.placementObject);
            CustomLog.Instance.InfoLog($"Creating PlacementObjectUiItem, SetCorrespondingObject: {placementObjectSo.placementObject}");
            
            instantiatedUiItemElement.CorrespondingPlacementObjectSO = placementObjectSo;
            CustomLog.Instance.InfoLog($"Creating PlacementObjectUiItem: {placementObjectSo.placementObject.name}");
            
            instantiatedUiItemElement.gameObject.name = placementObjectSo.placementObject.name;

            if (!placementObjectSo.GetEnableAmountInInventory())
            {
                instantiatedUiItemElement.EnableDisableAmountIndicatorText(false);
            }
            else
            {
                instantiatedUiItemElement.SetAmountOfInventory(placementObjectSo.GetAmountInInventory());
            }
            
            
            return instantiatedUiItemElement;
        }
        
        void ShowItemsOnCurrentPage()
        {
            CustomLog.Instance.InfoLog("ShowItemsOnCurrentPage: Showing Items on current Page. Current Page =" + currentPage);
            
            foreach (var kvp in pageToUIObjects)
            {
                CustomLog.Instance.InfoLog("ShowItemsOnCurrentPage: pageToUIObjects: " + kvp.Item2 + "has position:" + kvp.Item1);
                if (kvp.Item1 == currentPage)
                {
                    kvp.Item2.gameObject.SetActive(true);
                }
                else
                {
                    CustomLog.Instance.InfoLog("ShowItemsOnCurrentPage: Should set object to false: " + kvp.Item2.gameObject.name);
                    kvp.Item2.gameObject.SetActive(false);
                }     
            }
        }


        void DebugPrintDictionary()
        {
            foreach (var kvp in pageToUIObjects)
            {
                CustomLog.Instance.InfoLog("Page: " + kvp.Item1 + " Contains: " + kvp.Item2.name);
            }
        }

        int CalculatePage(int itemIndex)
        {
            return (int)Math.Ceiling((double)itemIndex / ItemsPerPage);
        }

        public void PageUp()
        {
            currentPage += 1;
            currentPage %= maxPage+1;
        

            if (currentPage == 0)
                currentPage = 1;
        }


        public void HideUIElements()
        {
            if (setManageObjectType == ManageObjectType.inventory)
            {
                CloseInventory();
            }
            
            HideInventoryButton();
            
            foreach (var placementObjectUI in _allItems)
            {
                if(placementObjectUI.GetComponent<HideUIElement>()) return;
            
                placementObjectUI.transform.gameObject.SetActive(false);
            }
        
            DeselectAllElements();

            barIsHidden = true;
        }

        void HideInventoryButton()
        {
            inventoryUiItem.gameObject.SetActive(false);

            //
            //uiManager.HideUI();
        }

        void ShowInventoryButton()
        {
            inventoryUiItem.gameObject.SetActive(true);

            //
            //uiManager.ShowUI();
        }

        public void ShowUIElements()
        {
            ShowInventoryButton();
            
            foreach (var placementObjectUI in _allItems)
            {
                if(placementObjectUI.GetComponent<HideUIElement>()) return;

                placementObjectUI.transform.gameObject.SetActive(true);

            }
        
            barIsHidden = false;
        }

        public void DeselectAllElements()
        {
            foreach (var placementObjectUI in _allItems)
            { 
                if(placementObjectUI == null) continue;
                
                
                if(placementObjectUI.GetComponent<HideUIElement>()) continue;
                placementObjectUI.HideSelectedState();
            }
        
            OnUiElementSelected?.Invoke(null);
            selectedUiItem = null;
        }

        void DeselectOnPlacement()
        {
            OnUiElementSelected?.Invoke(null);
            DeselectAllElements();
        }

        private PlacementObjectUiItem selectedUiItem = null;
        private const float minimalMoveThreshold = 50f;
        Vector2 initalInputPos = Vector2.zero;


        bool isCurrentlyOverPlacementBar = false;
        
        //called in update
        void CheckForSelectUIElement()
        {
            if(EventSystem.current == null) return;

            Vector2 inputPos = Vector2.zero;
            
            bool isInputDetected = false;
            
            // Check for mouse input
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                // isInitialDragPositionOverPlacementBar = IsPointerOverPlacementBar(inputPos);
                CustomLog.Instance.InfoLog("TouchPhase => Began");
                inputPos = Input.mousePosition;
                isInputDetected = true;
            }
            if (isInventoryOpen  && selectedUiItem != null)
            {
                if((Input.GetMouseButton(0) ||  (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)) && _allItems.Contains(selectedUiItem))
                {
                    _scrollRect.enabled = false;
                    
                    inputPos = Input.mousePosition;


                    if (initalInputPos == Vector2.zero)
                    {
                        initalInputPos = inputPos;
                    }
                    
                    
                    CustomLog.Instance.InfoLog("inputPosMag: "+ inputPos.magnitude);
                    CustomLog.Instance.InfoLog("initalInputMag: " + initalInputPos.magnitude);
                    
                    CustomLog.Instance.InfoLog("Input Pos difference: " + (inputPos.magnitude - initalInputPos.magnitude));

                    if (Vector2.Distance(inputPos, initalInputPos) > minimalMoveThreshold)
                    {
                        // if (isDraggingItem == false)
                        // {
                        // }
                        selectedUiItem.ChangeParent(dragCanvas);
                        selectedUiItem.MoveItem(inputPos);
                    }
                    

                    CustomLog.Instance.InfoLog("TouchPhase => Moved");
                    isDraggingItem = true;
                }
                else if ((Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Canceled)) && selectedUiItem != null && _allItems.Contains(selectedUiItem))
                {
                    _scrollRect.enabled = true;
                    
                    inputPos = Input.mousePosition;
                    isCurrentlyOverPlacementBar = IsPointerOverPlacementBar(inputPos);

                    CustomLog.Instance.InfoLog("Is currently over placementbar => " + isCurrentlyOverPlacementBar);


                    PlacementObjectUiItem item = selectedUiItem;
                    _allItems.Remove(selectedUiItem);

                    CustomLog.Instance.InfoLog("Should change in between placement bar and inventory");
                    if (isCurrentlyOverPlacementBar)
                    {
                        CustomLog.Instance.InfoLog("Setting item: " + item.name + "to index 0");
                        EnsureListCapacity(_allItems, 1);
                        _allItems.Insert(0,item);
                        item.ChangeParent(uiObjectParent);
                    }
                    else
                    {
                        CustomLog.Instance.InfoLog("Setting item: " + item.name + "to index" + 
                                                 (ItemsPerPage + 1));
                        EnsureListCapacity(_allItems, ItemsPerPage + 1);
                        _allItems.Insert(ItemsPerPage + 1, item);
                    }
                
                    AdjustUIBarInventoryItems();
                    DeselectAllElements();
                    isDraggingItem = false;
                    initalInputPos = Vector2.zero;
                    
                    ReloadUIElements();
                    OnUiElementsChanged?.Invoke();
                } 
                
                isCurrentlyOverPlacementBar = false;

            }

            
            // Proceed only if an input is detected
            if (isInputDetected)
            {
                if (selectedUiItem != null && _inventoryItems.Contains(selectedUiItem) && isDraggingItem)
                {
                    //move item along
                    selectedUiItem.MoveItem(inputPos);
                    CustomLog.Instance.InfoLog("Should move item");
                }
                
                //Is over UI Object ?
                if (EventSystem.current.IsPointerOverGameObject() || // For mouse
                    (Input.touchCount > 0 &&
                     EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))) // For touch
                {
                    PointerEventData pointerEventData = new PointerEventData(EventSystem.current) { position = inputPos };
                    List<RaycastResult> results = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(pointerEventData, results);
                
                    foreach (var raycastResult in results)
                    {
                        selectedUiItem = raycastResult.gameObject.GetComponentInParent<PlacementObjectUiItem>();
                    
                        if(selectedUiItem == null) continue;
                        
                        if (selectedUiItem.TryGetComponent(out InventoryUIElement inventoryUIElement))
                        {
                            CustomLog.Instance.InfoLog("selectedUiItem.TryGetComponent Inventory: " + inventoryUIElement.name);
                            
                            if(!inventoryUIElement) return;
                                
                            // !uiInventoryObject) return;
                            
                            if (isInventoryOpen)
                            {
                                CloseInventory();
                            }
                            else
                            {
                                OpenInventory();
                            }
                            // ReloadUIElements();
                        }

                        if (selectedUiItem.TryGetComponent(out PaginationUIElement paginationUIElement))
                        {
                            PageUp();
                            ShowItemsOnCurrentPage();
                        }
                    
                        if (selectedUiItem.TryGetComponent(out HideUIElement hidePlacementBarUI))
                        {
                            if (barIsHidden)
                            {
                                CustomLog.Instance.InfoLog("Show Elements Outter");
                                ShowUIElements();
                            }
                            else
                            {
                                CustomLog.Instance.InfoLog("Hide Elements Outter");
                                HideUIElements();
                            }
                        }

                        if (selectedUiItem != null)
                        {
                            if(selectedUiItem.IsDisabled())
                                return;
                        
                            if (_allItems.Contains(selectedUiItem))
                            {
                                if (selectedUiItem.IsActive())
                                {
                                    OnUiElementSelected?.Invoke(null);
                                    // OnUiElementSelectedSO?.Invoke(null);
                                    selectedUiItem.HideSelectedState();
                                }
                                else
                                {
                                    selectedUiItem.ShowSelectedState();
                                    OnUiElementSelected?.Invoke(selectedUiItem.GetCorrespondingObject());
                                    // OnUiElementSelectedSO?.Invoke(selectedUiItem.CorrespondingPlacementObjectSO);
                                
                                
                                    foreach (var objects in _allItems)
                                    {
                                        if(objects == null) continue;
                                        
                                        if (objects != selectedUiItem)
                                        {
                                            objects.HideSelectedState();
                                        }
                                    }
                                } 
                                break;
                            } 
                        }
                    }
                }
            }
        }
        
        
        void EnsureListCapacity<T>(List<T> list, int capacity)
        {
            while (list.Count < capacity)
            {
                // Add default values (e.g., 0 for int, null for objects)
                list.Add(default(T));
            }
        }
    }
}

public enum PlacementBarPosition
{
    bottom,
    left,
    right,
    top
}


public enum ManageObjectType
{
    inventory, 
    paginate
}
