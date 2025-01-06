using System.Collections.Generic;
using ARMagicBar.Resources.Scripts.Debugging;
using ARMagicBar.Resources.Scripts.PlacementBar;
using UnityEngine;

namespace ARMagicBar.SampleScenes.PhysicsTower
{
    public class RemoveObjectsFromInventoryAtRuntimeList : MonoBehaviour
    {
        [SerializeField] private List<PlaceableObjectSODatabase> databasesWhereToReduceAndRemoveObjects;
        [SerializeField] private bool resetInventoryAmount;


        private Dictionary<PlacementObjectSO, int> objectToInitialInventory = new();

        void Start()
        {
            //Listen to the OnObjectSpawnedWithSO event. Please take a look at the
            //API to find an overview of the most important events
            ARPlacementPlaneMesh.Instance.OnObjectSpawnedWithSO += OnObjectSpawnedWithSO;
            SetInitialInventories();
        }

        //Remove one unit from the inventory on placed, when the inventory is empty, grey it out. 
        private void OnObjectSpawnedWithSO(PlacementObjectSO placementObject, GameObject gameObjectRef)
        {
            placementObject.SetAmountInInventory(placementObject.GetAmountInInventory() - 1);

            if (placementObject.GetAmountInInventory() == 0)
            {
                placementObject.SetItemEnableToSelectInUI(false);
            }
        }

        //In the beginning of the scene we will store each inventory value to later on reset it.
        void SetInitialInventories()
        {
            foreach (var database in databasesWhereToReduceAndRemoveObjects)
            {
                foreach (var placementObject in database.PlacementObjectSos)
                {
                    objectToInitialInventory.Add(placementObject, placementObject.GetAmountInInventory());
                    placementObject.SetItemEnableToSelectInUI(true);
                }
            }
        }

        //Resetting the amount of the placeable objects in the end. As we are using scriptable objects, reducing the inventory amount 
        //in-game will be stored even after the game scene has been ended. 
        void ResetInitialInventories()
        {
            foreach (var database in databasesWhereToReduceAndRemoveObjects)
            {
                foreach (var placementObject in database.PlacementObjectSos)
                {
                    placementObject.SetAmountInInventory(objectToInitialInventory[placementObject]);
                    placementObject.SetItemEnableToSelectInUI(false);
                }
            }
        }

        private void OnDestroy()
        {
            CustomLog.Instance.InfoLog("");
            ResetInitialInventories();
        }
    }
}
