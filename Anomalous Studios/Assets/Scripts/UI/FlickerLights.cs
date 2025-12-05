using UnityEngine;
using System.Collections;

public class FlickerLights : MonoBehaviour
{
    [SerializeField] Light lightSource;
    public float minTime;
    public float maxTime;

    void Start()
    {
        if (lightSource == null)
            lightSource = GetComponent<Light>();

        StartCoroutine(Flicker());
    }

    /// <summary>
    /// Turns on and off lights at random intervals
    /// </summary>
    /// <returns></returns>
    IEnumerator Flicker()
    {
        while (true)
        {
            lightSource.enabled = !lightSource.enabled;
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));
        }
    }
}
