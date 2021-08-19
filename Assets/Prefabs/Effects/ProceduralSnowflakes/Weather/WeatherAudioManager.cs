using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherAudioManager : MonoBehaviour
{
    public WeatherManager weatherMgr;

    public Rain rain; // use actual rain object so the audio matches the visual effect

    public AudioSource windLow, windHigh, rainLow, rainHigh, thunder;

    private float thunderTimer = 0.0f;

    // Use this for initialization
    void Start()
    {
        thunderTimer = Random.Range(15.0f, 25.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (rain.m_RainIntensity <= 0.25f)
        {
            rainLow.volume = Mathf.Lerp(0, 1, rain.m_RainIntensity / 0.25f);
            rainHigh.volume = 0.0f;
        }
        else if (rain.m_RainIntensity <= 0.75f)
        {
            rainLow.volume = Mathf.Lerp(1, 0, (rain.m_RainIntensity - 0.25f) / 0.5f);
            rainHigh.volume = Mathf.Lerp(0, 1, (rain.m_RainIntensity - 0.25f) / 0.75f);
        }
        else
        {
            rainLow.volume = 0.0f;
            rainHigh.volume = Mathf.Lerp(0, 1, (rain.m_RainIntensity - 0.25f) / 0.75f);
            thunderTimer -= Time.deltaTime;

            if (thunderTimer <= 0.0f)
            {
                thunder.pitch = Random.Range(0.8f, 1.2f);
                thunder.Play();
                thunderTimer = Random.Range(15.0f, 25.0f);
            }
        }

        if (weatherMgr.windStrength <= 0.25f)
        {
            windLow.volume = Mathf.Lerp(0, 1, weatherMgr.windStrength / 0.25f);
            windHigh.volume = 0.0f;
        }
        else if (weatherMgr.intensity <= 0.75f)
        {
            windLow.volume = Mathf.Lerp(1, 0, (weatherMgr.windStrength - 0.25f) / 0.5f);
            windHigh.volume = Mathf.Lerp(0, 1, (weatherMgr.windStrength - 0.25f) / 0.75f);
        }
        else
        {
            windLow.volume = 0.0f;
            windHigh.volume = Mathf.Lerp(0, 1, (weatherMgr.windStrength - 0.25f) / 0.75f);
        }
    }
}
