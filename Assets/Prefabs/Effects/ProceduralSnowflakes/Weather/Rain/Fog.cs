using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : MonoBehaviour {
    [Range(0.0f, 1.0f)]
    public float m_Intensity = 0.0f;
    private float m_IntensityCheck;
    ParticleSystem m_FogCache;
    public int m_MaxParticles = 2000;

    private new BoxCollider collider;

    // Use this for initialization
    [System.Obsolete]
    void Start () {
        m_IntensityCheck = m_Intensity;
        m_FogCache = GetComponent<ParticleSystem>();
        m_FogCache.emissionRate = (int)(m_MaxParticles * m_Intensity * 0.1f);
        collider = GetComponent<BoxCollider>();
    }

    [System.Obsolete]
    public void SetIntensity(float a_Intensity)
    {
        //m_FogCache.maxParticles = (int)(m_MaxParticles * a_Intensity);
        m_FogCache.emissionRate = (int)(m_MaxParticles * a_Intensity * 0.1f);
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update () {

        if (m_Intensity != m_IntensityCheck)
        {
            SetIntensity(m_Intensity);
            m_IntensityCheck = m_Intensity;
        }
	}

    public void setEmitterPosition(Vector3 pos)
    {
        Vector3 offset = pos - transform.position;
        var shape = m_FogCache.shape;
        shape.position = offset;
        collider.center = offset;
    }
}
