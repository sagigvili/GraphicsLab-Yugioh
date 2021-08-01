using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingsLoop : MonoBehaviour
{
    private int timer = 0;
    // Update is called once per frame
    void Update()
    {
        if (timer%100 == 0)
        {
            ParticleSystem ps = transform.GetChild(Random.Range(0, transform.childCount)).GetComponent<ParticleSystem>();
            ps.Play();
        }
        timer++;
    }
}
