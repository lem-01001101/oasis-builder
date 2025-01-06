using ARMagicBar.Resources.Scripts.PlacementBar;
using UnityEngine;

namespace ARMagicBar.Resources.Scripts.PlacementObjects
{
    public class ReferenceToSO : MonoBehaviour
    {
        [HideInInspector]
        public PlaceableObjectSODatabase correspondingDatabase;
        [HideInInspector]
        public PlacementObjectSO correspondingObject;
    

        public void SetPlacementObjectSO(PlacementObjectSO objectSO)
        {
            correspondingObject = objectSO;
        }

        public void SetCorrespondingDatabaseSO(PlaceableObjectSODatabase database)
        {
            correspondingDatabase = database;
        }
    
        public PlacementObjectSO GetPlacementObejctSO()
        {
            return correspondingObject;
        } 
    
        public PlacementObjectSO CorrespondingObject
        {
            get;
            set;
        }

    }
}
