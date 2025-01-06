using ARMagicBar.Resources.Scripts.PlacementBar;
using UnityEngine;

namespace ARMagicBar.SampleScenes.PickUpAndPlace
{
    public class RemoveObjectFromInventoryOnPlaced : MonoBehaviour
    {
    
        [SerializeField] private PlaceableObjectSODatabase placeableObjectSoDatabase; 
    

        void Start()
        {
            ARPlacementPlaneMesh.Instance.OnObjectSpawnedWithSO += InstanceOnOnObjectSpawnedWithSO;
        }

        private void InstanceOnOnObjectSpawnedWithSO(PlacementObjectSO obj, GameObject objectRef)
        {
            foreach (var placeableobject in placeableObjectSoDatabase.PlacementObjectSos)
            {
                if (placeableobject == obj && obj.GetEnableAmountInInventory())
                {
                    obj.SetAmountInInventory(obj.GetAmountInInventory() - 1);
        
                    if (obj.GetAmountInInventory() == 0)
                    {
                        obj.SetIsEnabledInUI(false);
                    }
                }
            }

        }
    }
}
