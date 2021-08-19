using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snow : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float m_Intensity = 0.0f;
    private float m_IntensityCheck;
    ParticleSystem m_SnowCache;
    public int m_MaxParticles = 20000;
    private new SphereCollider collider;

    public List<Sprite> snowflakeSprites;

    [System.Obsolete]
    public void SetIntensity(float a_SnowIntensity)
    {
        m_SnowCache.emissionRate = (int)(m_MaxParticles * a_SnowIntensity);
    }

    // Use this for initialization
    [System.Obsolete]
    void Start()
    {
        addSnowFlakesToParticle();
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        if (m_Intensity != m_IntensityCheck)
        {
            SetIntensity(m_Intensity);
            m_IntensityCheck = m_Intensity;
        }
    }

    public void setEmitterPosition(Vector3 pos)
    {
        Vector3 offset = pos - transform.position;
        var shape = m_SnowCache.shape;
        shape.position = offset;
        collider.center = offset;
    }

    public void addSprite(Sprite s)
    {
        snowflakeSprites.Add(s);
    }

    [System.Obsolete]
    public void addSnowFlakesToParticle()
    {
        m_IntensityCheck = m_Intensity;
        m_SnowCache = GetComponent<ParticleSystem>();
        m_SnowCache.emissionRate = (int)(m_MaxParticles * m_Intensity);
        collider = GetComponent<SphereCollider>();
        foreach (Sprite s in snowflakeSprites)
        {
            m_SnowCache.textureSheetAnimation.AddSprite(s);
        }
    }

    public void ChangeAlbedo(Texture2D text)
    {
        GetComponent<Renderer>().material.SetTexture("_MainTex", text);
    }
}
