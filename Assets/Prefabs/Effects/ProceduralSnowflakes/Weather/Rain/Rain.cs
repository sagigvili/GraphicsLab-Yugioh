using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour {

    public float m_RadiusOfCloud;
    [Range(0.0f, 1.0f)]
    public float m_RainIntensity;
    private float m_RainIntensityCheck;
    ParticleSystem m_Rain;
    float m_EmissionRate;
    ParticleSystem.EmissionModule m_Emission;
    public int m_MaxParticles=20000;

    [System.Obsolete]
    public void SetIntensity(float a_RainIntensity)
    {
        m_Rain.maxParticles = (int)(m_MaxParticles * a_RainIntensity);
        m_Rain.emissionRate = (int)(m_MaxParticles * a_RainIntensity);
    }

    // Use this for initialization
    [System.Obsolete]
    void Start () {
        m_RainIntensityCheck = m_RainIntensity;
        m_Rain = gameObject.GetComponent<ParticleSystem>();
        m_Rain.emissionRate = (int)(m_MaxParticles * m_RainIntensity);
        m_EmissionRate = m_Rain.emissionRate;
        m_Rain.maxParticles = (int)(m_MaxParticles * m_RainIntensity);
        m_Emission = GetComponent<ParticleSystem>().emission;
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update () {
        if (m_RainIntensity != m_RainIntensityCheck)
        {
            SetIntensity(m_RainIntensity);
            m_RainIntensityCheck = m_RainIntensity;
        }
    }

    public void setEmitterPosition(Vector3 pos)
    {
        Vector3 offset = pos - transform.position;
        var shape = m_Rain.shape;
        shape.position = offset;
    }
}
