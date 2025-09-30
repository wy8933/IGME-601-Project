using System.Collections;
using UnityEngine;

public class LightDetection : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private LayerMask lightHitMask; //What can light hit? Player, enemies, walls, objects, etc
    [Tooltip("The layers that will block the lights")]
    [SerializeField] private LayerMask occluderMask;
    private RaycastHit lightHitInfo; //Create one variable to hold hit info instead of many
    private float targetLightTotal; //Target light value
    [SerializeField] private bool applySmoothing = true; //Smoothly change light value or not
    [SerializeField] private float lightResponsiveness = 10f; //Time value for lerping to target light value
    public float lightTotal; //Amount of light hitting player, from 0 to 1

    [Min(0.25f)]
    public float lightDetectionCooldown;

    const float Skin = 0.05f;

    public void Start()
    {
        StartCoroutine(CheckLightValueLoop());
    }

    private IEnumerator CheckLightValueLoop()
    {
        var wait = new WaitForSeconds(lightDetectionCooldown);
        while (true)
        {
            float target = 0f;

            var lights = LightManager.Instance?.Lights;
            if (lights != null && lights.Length > 0)
            {
                foreach (var light in lights)
                {
                    if (!light || !light.enabled || !light.gameObject.activeInHierarchy) continue;

                    switch (light.type)
                    {
                        case LightType.Point: 
                            target += CheckPointLight(light); 
                            break;
                        case LightType.Spot: 
                            target += CheckSpotLight(light); 
                            break;
                    }
                }
            }

            lightTotal = applySmoothing
                ? Mathf.Lerp(lightTotal, target, Time.deltaTime * lightResponsiveness)
                : target;

            lightTotal = Mathf.Clamp01(lightTotal);
            yield return wait;
        }
    }

    #region LightCheck

    float CheckPointLight(Light pl)
    {
        Vector3 toPlayer = transform.position - pl.transform.position;
        float dist = toPlayer.magnitude;

        if (dist > pl.range) return 0f;
        if (IsBlocked(pl.transform.position, transform.position)) return 0f;

        return LightValueCalculation(pl.intensity, toPlayer.sqrMagnitude);
    }

    private float CheckSpotLight(Light _spotLight)
    {
        Vector3 direction = transform.position - _spotLight.transform.position;
        float distance = direction.magnitude;

        if (distance > _spotLight.range) return 0f;

        float angle = Vector3.Angle(_spotLight.transform.forward, direction);
        if (angle > _spotLight.spotAngle * 0.5f) return 0f;

        if (IsBlocked(_spotLight.transform.position, transform.position)) return 0f;

        return LightValueCalculation(_spotLight.intensity, direction.sqrMagnitude);
    }
    #endregion

    bool IsBlocked(Vector3 from, Vector3 to)
    {
        Vector3 dir = to - from;
        float dist = dir.magnitude;
        if (dist < 0.0001f) return false;
        return Physics.Raycast(from, dir / dist, dist - 0.05f, occluderMask, QueryTriggerInteraction.Ignore);
    }

    float LightValueCalculation(float intensity, float sqrDistance)
    {
        return Mathf.Clamp01(intensity / Mathf.Max(1f, sqrDistance));
    }
}