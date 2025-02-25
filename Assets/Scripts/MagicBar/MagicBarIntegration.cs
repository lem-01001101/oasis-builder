using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARMagicBar.Resources.Scripts.PlacementObjects;
using ARMagicBar.Resources.Scripts.PlacementBar;
using System;
using UnityEditor;


public class MagicBarIntegration : MonoBehaviour
{

    public TemperatureManager temperatureManager;

    // Start is called before the first frame update
    void Start()
    {
        //MenuUILogic isGameOn = FindObjectOfType<MenuUILogic>();
        //if(isGameOn)
        //{
            ARPlacementPlaneMesh.Instance.OnObjectSpawnedWithSO += OnObjectSpawned;
        //}
    }

    private void OnObjectSpawned(PlacementObjectSO objectData, GameObject spawnedObject)
    {
        Debug.Log($"Object spawned: {spawnedObject.name}");
        string change_temp = "0";

        // Example: Hardcoded temperature impact for specific objects
        if (spawnedObject.name.Contains("SunshinPalmTreeParent_Placeable"))
        {
            temperatureManager.AdjustTemperature(-1.5f);
            change_temp = "-1.5";
            Debug.Log("Planted");
        }
        else if (spawnedObject.name.Contains("BloodwoodTreeParent_Placeable"))
        {
            temperatureManager.AdjustTemperature(-0.5f);
            change_temp = "-0.5";
            Debug.Log("Planted");

        }
        else if (spawnedObject.name.Contains("Canopy1Parent_Placeable"))
        {
            temperatureManager.AdjustTemperature(-3.0f);
            change_temp = "-3";
            Debug.Log("Planted");

        }
        else if (spawnedObject.name.Contains("Grass1Parent_Placeable"))
        {
            temperatureManager.AdjustTemperature(-2.0f);
            change_temp = "-2";
            Debug.Log("Planted");
        }
        else if (spawnedObject.name.Contains("PondParent_Placeable"))
        {
            temperatureManager.AdjustTemperature(-2.0f);
            change_temp = "-1";
            Debug.Log("Planted");
        }
        //more cases or retrieve metadata dynamically
        //FindObjectOfType<NotificationManager>().ShowNotification("Temperature has been reduced by " + change_temp + "°C");
        //temperatureManager.UpdateTemperatureUI();
        Debug.Log($"Change temp:{change_temp}");

    }
}
