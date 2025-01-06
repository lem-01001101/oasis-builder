using System;
using System.Collections.Generic;
using ARMagicBar.Resources.Scripts.Debugging;
using ARMagicBar.Resources.Scripts.PlacementBarUI;
using ARMagicBar.Resources.Scripts.TransformLogic;
using UnityEditor;
using UnityEngine;

namespace ARMagicBar.Resources.Scripts.PlacementBar
{
    public class PlacementBarLogic : MonoBehaviour
    {
        [SerializeField] private List<PlaceableObjectSODatabase> databases = new();
        [SerializeField] private List<(PlacementObjectSO, PlaceableObjectSODatabase)> placementObjects = new();
    
        private TransformableObject objectToPlace;
        private PlacementObjectSO placementObjectSoToPlace;

        [HideInInspector]
        public static PlacementBarLogic Instance;

        public event Action OnDatabaseChanged;
    
        void Awake()
        {
            Instance = this;
            objectToPlace = null;
            SetPlacementObjects();
        }
        
        void SetPlacementObjects()
        {
            foreach (var database in databases)
            {
                database.CleanPlacementObjectSos();
                
                foreach (var placementObject in database.PlacementObjectSos)
                {
                    placementObjects.Add((placementObject, database));
                }
            }
        }

        private void RefreshDatabase()
        {
            PlacementBarUIElements.Instance.ReloadUIElements();
        }

        private void Start()
        {
            PlacementBarUIElements.Instance.OnUiElementSelected += SetObjectToInstantiate;
        }

        private void OnDestroy()
        {
            PlacementBarUIElements.Instance.OnUiElementSelected -= SetObjectToInstantiate;
        }

        private void SetObjectToInstantiate(TransformableObject obj)
        {
            if (obj != null)
            {
                CustomLog.Instance.InfoLog("Should set object to place to " + obj.name);
            }
            else
            {
                CustomLog.Instance.InfoLog("Should set object to null");
            }
            objectToPlace = obj;
        }

        
        //Returns the object that is currently selected "ToPlace".
        //Return null if nothing is selected
        public TransformableObject GetPlacementObject()
        {
            return objectToPlace;
        }

        
        //Returns the scriptable object for the object that is currently selected "ToPlace".
        //Return null if nothing is selected
        public PlacementObjectSO GetPlacementObjectSo()
        {
            return placementObjectSoToPlace;
        }
        
        //Returns all placement objects including their corresponding database
        public List<(PlacementObjectSO, PlaceableObjectSODatabase)> GetAllObjects()
        {
            if (placementObjects == null) return default;
        
            return placementObjects;
        }

        // public bool GetIsSelectedFromUI(PlacementObjectUiItem uiItemObject)
        // {
        //     foreach (var placementObjectSo in placementObjects)
        //     {
        //         if (placementObjectSo.uiSprite == uiItemObject)
        //         {
        //             return default;
        //         }
        //     }
        //     return default;
        // }
    
    }
}
