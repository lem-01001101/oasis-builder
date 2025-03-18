using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TemperatureManager : MonoBehaviour
{

    public TextMeshProUGUI temperatureText;
    //public TextMeshProUGUI latitudeText;
    //public TextMeshProUGUI longitudeText;

    public TextMeshProUGUI humidityText;
    public TextMeshProUGUI cloudcoverText;
    public TextMeshProUGUI windspeedText;

    public TextMeshProUGUI oasisTempText;
    public TextMeshProUGUI oasisDifferenceText;

    public TextMeshProUGUI menuCurrentTempText;
    public TextMeshProUGUI menuOasisDifferenceText;


    // testing
    private float currentTemperature = 75.0f; // Example starting temperature
    
    private float latitude;
    private float longitude;
    private float totalDelta = 0;

    private string apiUrl = "https://api.open-meteo.com/v1/forecast";


    /********
    public float simulatedLatitude = 34.1592f;  // Example latitude
    public float simulatedLongitude = -118.5012f;  // Example longitude

    private bool isLocationEnabled;
    */

    void Start()
    {

        StartCoroutine(GetLocationAndTemperature());
    }

    // Update is called once per frame
    /*
    void Update()
    {
        
    }*/


    IEnumerator GetLocationAndTemperature()
    {
        // Start location service
        
        if (!Input.location.isEnabledByUser)
        {
            temperatureText.text = "Err1"; 
            Debug.Log("Location services not enabled");
            yield break;
        }

        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in time
        if (maxWait <= 0)
        {
            temperatureText.text = "Err2";
            Debug.Log("Timed out");
            yield break;
        }
        

        // Connection has failed
        
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            temperatureText.text = "Err3";
            Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            Debug.Log($"Latitude: {latitude}, Longitude: {longitude}");

            Input.location.Stop();

            // Fetch temperature data
            string url = $"{apiUrl}?latitude={latitude}&longitude={longitude}&current=temperature_2m,relative_humidity_2m,cloud_cover,wind_speed_10m&temperature_unit=fahrenheit&wind_speed_unit=ms";
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                temperatureText.text = "Err4";
                Debug.Log("Error fetching temperature data");
            }
            else
            {
                // Parse JSON response
                var jsonResponse = request.downloadHandler.text;
                OpenMeteoResponse weatherInfo = JsonUtility.FromJson<OpenMeteoResponse>(jsonResponse);
                Debug.Log($"returned JSON: {jsonResponse}");

                if (weatherInfo != null && weatherInfo.current != null)
                {
                    Debug.Log($"Parsed JSON: {weatherInfo.current}");
                    float temp = weatherInfo.current.temperature_2m;
                    //float 
                    temperatureText.text = $"{temp}°F";
                    currentTemperature = temp;
                    Debug.Log($"Testing temperature: {temp}");

                    float humidity = weatherInfo.current.relative_humidity_2m;
                    float cloudcover = weatherInfo.current.cloud_cover;
                    float windspeed = weatherInfo.current.wind_speed_10m;

                    humidityText.text = $"{humidity}%";
                    cloudcoverText.text = $"{cloudcover}%";
                    windspeedText.text = $"{windspeed} mph";

                }
                else
                {
                    temperatureText.text = "Err5";
                    Debug.Log("Temperature data unavailable");
                }
            }
        }
    }

    public void AdjustTemperature(float delta)
    {
        //currentTemperature += delta;
        totalDelta += delta;
        UpdateTemperatureUI();
        Debug.Log($"Temperature adjusted by {delta}°F. Current Temperature: {currentTemperature}°F");
    }

    public void UpdateTemperatureUI()
    {
        //temperatureText.text = $"Temperature: {currntTemperature}°F";
        float totalOasisDifference = currentTemperature + totalDelta;
        oasisDifferenceText.text = $"{totalDelta}°F";
        oasisTempText.text = $"{currentTemperature}°F{totalDelta}°F = {totalOasisDifference}°F";

        menuCurrentTempText.text = $"{currentTemperature}°F";
        menuOasisDifferenceText.text = $"{totalOasisDifference}°F";

    }

    [System.Serializable]
    private class OpenMeteoResponse
    {
        public CurrentWeather current;
    }

    [System.Serializable]
    private class CurrentWeather
    {
        public float temperature_2m;

        public float relative_humidity_2m;
        public float wind_speed_10m;

        public float cloud_cover;


    }
}
