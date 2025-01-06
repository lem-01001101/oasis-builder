using System;
using System.Collections.Generic;
using ARMagicBar.Resources.Scripts.Debugging;
using UnityEngine;

namespace ARMagicBar.Resources.Scripts.PlacementBar
{
    [CreateAssetMenu(fileName = "PlaceableObjectDatabase", 
        menuName = "ARMagicBar/PlaceableObjectDatabase", order = 1)]
    public class PlaceableObjectSODatabase : ScriptableObject
    {
        [SerializeField] public string DatabaseName = "Database";
        // [SerializeField] public string SaveDirectory = "Assets/ARMagicBar/Resources/PlaceableObjects";
        public List<PlacementObjectSO> PlacementObjectSos = new List<PlacementObjectSO>();



        public void CleanPlacementObjectSos()
        {
            for (int i = 0; i < PlacementObjectSos.Count; i++)
            {
                if (PlacementObjectSos[i] == null)
                {
                    Debug.LogWarning($"The PlaceableObjectSODatabase {DatabaseName} had a missing object which was removed.");
                }
            }
        }
    }
}