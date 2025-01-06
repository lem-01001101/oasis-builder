using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARMagicBar.Resources.Scripts.PlacementObjects;
using ARMagicBar.Resources.Scripts.PlacementBar;
using System;


public class MagicBarIntegration : MonoBehaviour
{

    public TemperatureManager temperatureManager;

    // Start is called before the first frame update
    void Start()
    {
        ARPlacementPlaneMesh.Instance.OnObjectSpawnedWithSO += OnObjectSpawned;
    }

    private void OnObjectSpawned(PlacementObjectSO objectData, GameObject spawnedObject)
    {
        Debug.Log($"Object spawned: {spawnedObject.name}");

        // Example: Hardcoded temperature impact for specific objects
        if (spawnedObject.name.Contains("WoodenBlockParent_Placeable"))
        {
            temperatureManager.AdjustTemperature(-1.5f);
            Debug.Log("Planted");
        }
        else if (spawnedObject.name.Contains("WoodenPlaneParent_Placeable"))
        {
            temperatureManager.AdjustTemperature(-0.5f);
            Debug.Log("Planted");

        }
        else if (spawnedObject.name.Contains("WoodenPlankParent_Placeable"))
        {
            temperatureManager.AdjustTemperature(-3.0f);
            Debug.Log("Planted");

        }
        else if (spawnedObject.name.Contains("WoodenRoofParent_Placeable"))
        {
            temperatureManager.AdjustTemperature(-2.0f);
            Debug.Log("Planted");
        }

        //more cases or retrieve metadata dynamically
    }
}
