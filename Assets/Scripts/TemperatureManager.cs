using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TemperatureManager : MonoBehaviour
{

    public TextMeshProUGUI temperatureText;
    public TextMeshProUGUI latitudeText;
    public TextMeshProUGUI longitudeText;
    
    private float latitude;
    private float longitude;

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
            temperatureText.text = "Location services not enabled";
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
            temperatureText.text = "Timed out";
            yield break;
        }
        

        // Connection has failed
        
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            temperatureText.text = "Unable to determine device location";
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            Debug.Log($"Latitude: {latitude}, Longitude: {longitude}");
            longitudeText.text = $"Longitude: {longitude}";
            latitudeText.text = $"Latitude: {latitude}";

            Input.location.Stop();

            // Fetch temperature data
            string url = $"{apiUrl}?latitude={latitude}&longitude={longitude}&current=temperature_2m&temperature_unit=fahrenheit";
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                temperatureText.text = "Error fetching temperature data";
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
                    temperatureText.text = $"Temperature: {temp}°F";
                    Debug.Log($"Testing temperature: {temp}");
                }
                else
                {
                    temperatureText.text = "Temperature data unavailable";
                }
            }
        }
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
        public float windspeed;
        public float winddirection;
        public int weathercode;
        public string time;
    }
}
