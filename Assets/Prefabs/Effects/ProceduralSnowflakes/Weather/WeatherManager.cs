using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public enum WeatherStates
    {
        Clear,
        Rain,
        Snow
    }

    public WeatherStates currentWeather;

    [Range(0.0f, 1.0f)]
    public float intensity = 0;

    [Range(0.0f, 1.0f)]
    public float fogIntensity = 0;

    [Range(0.0f, 1.0f)]
    public float windStrength = 0;
    [Range(0.0f, 360.0f)]
    public float windDirection = 0;

    [Header("Slip Thresholds")]
    public float rainThreshold;
    public float snowThreshold;

    private float targetRainIntensity, targetSnowIntensity;
    private Vector3 prevPos;
    private float skyboxLerp = 0;
    private Queue<float> speedSamples = new Queue<float>();
    private const int MAX_SAMPLES = 30;
    private float runningSum = 0;

    private const float WEATHER_CHANGE_SPEED = 0.1f;

    private Rain rain;
    private Fog fog;
    private Snow snow;
    private WindZone windZone;

    // Use this for initialization
    void Start()
    {
        rain = GetComponentsInChildren<Rain>(true)[0];
        fog = GetComponentsInChildren<Fog>(true)[0];
        snow = GetComponentsInChildren<Snow>(true)[0];
        windZone = GetComponentsInChildren<WindZone>(true)[0];
    }

    // Update is called once per frame
    void Update()
    {
        targetRainIntensity = 0.0f;
        targetSnowIntensity = 0.0f;

        switch (currentWeather)
        {
            case WeatherStates.Rain:
                targetRainIntensity = intensity;
                break;
            case WeatherStates.Snow:
                targetSnowIntensity = intensity;
                break;
            default:
                break;
        }

        fog.m_Intensity = fogIntensity;

        // update skybox transition
        if (fogIntensity > 0 || intensity > 0 && (currentWeather == WeatherStates.Rain || currentWeather == WeatherStates.Snow))
        {
            skyboxLerp = Mathf.Min(1, skyboxLerp + Time.deltaTime * WEATHER_CHANGE_SPEED);
        }
        else
        {
            skyboxLerp = Mathf.Max(0, skyboxLerp - Time.deltaTime * WEATHER_CHANGE_SPEED);
        }
        RenderSettings.skybox.SetFloat("_Blend", skyboxLerp);

        float delta = Time.deltaTime * WEATHER_CHANGE_SPEED;

        rain.m_RainIntensity += Mathf.Sign(targetRainIntensity - rain.m_RainIntensity) * Mathf.Min(delta, Mathf.Abs(targetRainIntensity - rain.m_RainIntensity));
        snow.m_Intensity += Mathf.Sign(targetSnowIntensity - snow.m_Intensity) * Mathf.Min(delta, Mathf.Abs(targetSnowIntensity - snow.m_Intensity));
        
        // make weather follow player
        try
        {
            snow.setEmitterPosition(Camera.main.transform.position + new Vector3(0, 4, 0));
            fog.setEmitterPosition(Camera.main.transform.position + new Vector3(0, 4, 0));
            rain.setEmitterPosition(Camera.main.transform.position + new Vector3(0, 20, 0));
        }
        catch (System.Exception e)
        {
            print(e);
        }

        if (prevPos == null)
        {
            try
            {
                prevPos = Camera.main.transform.position;
            }
            catch (System.Exception e)
            {
                print(e);
            }
            return;
        }
        else
        {
            try
            {
                float distanceDelta = Vector3.Distance(Camera.main.transform.position, prevPos);

                float speed = distanceDelta / Time.deltaTime;
                speedSamples.Enqueue(speed);
                runningSum += speed;

                if(speedSamples.Count > MAX_SAMPLES)
                {
                    float v = speedSamples.Dequeue();
                    runningSum -= v;
                }

                prevPos = Camera.main.transform.position;
            }
            catch (System.Exception e)
            {
                print(e);
            }
        }

        windZone.windMain = Mathf.Lerp(0, 60, windStrength);
        Vector3 rot = windZone.transform.rotation.eulerAngles;
        rot.y = windDirection;
        windZone.transform.rotation = Quaternion.Euler(rot);
    }
}
