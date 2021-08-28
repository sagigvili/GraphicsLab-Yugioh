using UnityEngine;
using System.Collections;

/// <summary>
/// Generates random "clouds" (or whatever you want the prefab to be) along an X-Z plane
/// using PerlinNoise. 
/// </summary>
public class CloudGenerator : MonoBehaviour
{

    /// <summary>
    /// Our "cloud" object. Could also be a "swarm," or... a box? Idk...
    /// </summary>
    public GameObject cloudPrefab;

    /// <summary>
    /// Width of the area to draw the clouds
    /// </summary>
    public float width = 100.0f;
    /// <summary>
    /// Depth of the area to draw the clouds
    /// </summary>
    public float depth = 100.0f;

    /// <summary>
    /// The minimum step to take when iterating through the width and height.
    /// Creates a more staggered effect, especially when random's involved.
    /// </summary>
    public float stepMin = 1.0f;
    /// <summary>
    /// The maximum step to take when iterating through the width and height.
    /// Creates a more staggered effect, especially when random's involved.
    /// </summary>
    public float stepMax = 1.0f;


    /// <summary>
    /// The Threshold that is checked against the output of the PerlinNoise.
    /// if the PerlinNoise value (pcheck) is below the threshold, the object will not be placed.
    /// </summary>
    [Range(0, 1)]
    public float threshold = 0.5f;
    /// <summary>
    /// Used in PerlinNoise as a modifier to the X-Z values passed in.
    /// </summary>
    public float noiseStrength = 10.0f;

    /// <summary>
    /// All this is doing is drawing out the plane in the editor.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 min = new Vector3(transform.position.x - width / 2, transform.position.y, transform.position.z - depth / 2);
        Vector3 max = new Vector3(transform.position.x + width / 2, transform.position.y, transform.position.z + depth / 2);
        Gizmos.DrawLine(min, new Vector3(max.x, min.y, min.z));
        Gizmos.DrawLine(new Vector3(max.x, min.y, min.z), max);
        Gizmos.DrawLine(max, new Vector3(min.x, max.y, max.z));
        Gizmos.DrawLine(new Vector3(min.x, max.y, max.z), min);
    }

    void Awake()
    {
        StartCoroutine(GenerateClouds());
    }

    /// <summary>
    /// Generates the clouds in the scene using a coroutine. 
    /// All clouds created are also hidden in the Inspector heirachy, to avoid clutter.
    /// </summary>
    /// <returns></returns>
    IEnumerator GenerateClouds()
    {
        for (float x = 0; x < width; x += Random.Range(stepMin, stepMax))
        {
            for (float z = 0; z < depth; z += Random.Range(stepMin, stepMax))
            {
                float pcheck = Mathf.PerlinNoise(x / width * noiseStrength, z / depth * noiseStrength);
                if (pcheck >= threshold)
                {
                    GameObject cloud =
                    GameObject.Instantiate(cloudPrefab,
                        new Vector3(x + (transform.position.x - width / 2),
                            transform.position.y,
                            z + (transform.position.z - depth / 2)),
                        Random.rotation) as GameObject;
                    cloud.transform.SetParent(this.transform);
                    cloud.hideFlags = HideFlags.HideInHierarchy;
                }
                yield return null;
            }
        }
    }
}