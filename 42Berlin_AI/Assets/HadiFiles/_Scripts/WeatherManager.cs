using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager instance;

    [Header("Elements")]
    [SerializeField]
    private GameObject[] weatherElements;

    public enum WeatherState
    {
        DayRainy,
        DayLightRain,
        DayCloudy,
        DayLightCloud,
        DaySnow,
        Sunny,
        NightRainy,
        NightLightRain,
        NightCloudy,
        NightLightCloud,
        NightSnow,
        Moonlight,
        NONE
    }
    private WeatherState currentWeatherState = WeatherState.NONE;

    void Awake()
    {
        if (instance!=null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            currentWeatherState = WeatherState.DayRainy;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeatherState = WeatherState.DayLightRain;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeatherState = WeatherState.DayCloudy;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentWeatherState = WeatherState.DayLightCloud;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentWeatherState = WeatherState.DaySnow;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            currentWeatherState = WeatherState.Sunny;
        }
        WeatherSelector(currentWeatherState);
    }

    public void WeatherSelector(WeatherState weatherState)
    {

        if (weatherState == WeatherState.DayRainy)
        {
            for (int i = 0; i < weatherElements.Length; i++)
            {
                if (i != 0)
                {
                    weatherElements[i].SetActive(false);
                }
            }
            weatherElements[0].SetActive(true);
        }
        else if (weatherState == WeatherState.DayLightRain)
        {
            for (int i = 0; i < weatherElements.Length; i++)
            {
                if (i != 1)
                {
                    weatherElements[i].SetActive(false);
                }
            }
            weatherElements[1].SetActive(true);
        }
        else if (weatherState == WeatherState.DayCloudy)
        {
            for (int i = 0; i < weatherElements.Length; i++)
            {
                if (i != 2)
                {
                    weatherElements[i].SetActive(false);
                }
            }
            weatherElements[2].SetActive(true);
        }
        else if (weatherState == WeatherState.DayLightCloud)
        {
            for (int i = 0; i < weatherElements.Length; i++)
            {
                if (i != 3)
                {
                    weatherElements[i].SetActive(false);
                }
            }
            weatherElements[3].SetActive(true);
        }
        else if (weatherState == WeatherState.DaySnow)
        {
            for (int i = 0; i < weatherElements.Length; i++)
            {
                if (i != 4)
                {
                    weatherElements[i].SetActive(false);
                }
            }
            weatherElements[4].SetActive(true);
        }
        else if (weatherState == WeatherState.Sunny)
        {
            for (int i = 0; i < weatherElements.Length; i++)
            {
                if (i != 5)
                {
                    weatherElements[i].SetActive(false);
                }
            }
            weatherElements[5].SetActive(true);
        }
        else if (weatherState == WeatherState.NightRainy)
        {
            for (int i = 0; i < weatherElements.Length; i++)
            {
                if (i != 6)
                {
                    weatherElements[i].SetActive(false);
                }
            }
            weatherElements[6].SetActive(true);
        }
        else if (weatherState == WeatherState.NightLightRain)
        {
            for (int i = 0; i < weatherElements.Length; i++)
            {
                if (i != 7)
                {
                    weatherElements[i].SetActive(false);
                }
            }
            weatherElements[7].SetActive(true);
        }
        else if (weatherState == WeatherState.NightCloudy)
        {
            for (int i = 0; i < weatherElements.Length; i++)
            {
                if (i != 8)
                {
                    weatherElements[i].SetActive(false);
                }
            }
            weatherElements[8].SetActive(true);
        }
        else if (weatherState == WeatherState.NightLightCloud)
        {
            for (int i = 0; i < weatherElements.Length; i++)
            {
                if (i != 9)
                {
                    weatherElements[i].SetActive(false);
                }
            }
            weatherElements[9].SetActive(true);
        }
        else if (weatherState == WeatherState.NightSnow)
        {
            for (int i = 0; i < weatherElements.Length; i++)
            {
                if (i != 10)
                {
                    weatherElements[i].SetActive(false);
                }
            }
            weatherElements[10].SetActive(true);
        }
        else if (weatherState == WeatherState.Moonlight)
        {
            for (int i = 0; i < weatherElements.Length; i++)
            {
                if (i != 11)
                {
                    weatherElements[i].SetActive(false);
                }
            }
            weatherElements[11].SetActive(true);
        }
        else if (weatherState == WeatherState.NONE)
        {
            for (int i = 0; i < weatherElements.Length; i++)
            {
                weatherElements[i].SetActive(false);
            }
        }
    }
}
