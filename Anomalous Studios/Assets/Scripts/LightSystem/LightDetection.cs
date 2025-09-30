using System.Collections;
using UnityEngine;

public class LightDetection : MonoBehaviour
{

    [Header("Light Settings")]
    [SerializeField] private LayerMask lightHitMask; //What can light hit? Player, enemies, walls, objects, etc
    private RaycastHit lightHitInfo; //Create one variable to hold hit info instead of many
    private float targetLightTotal; //Target light value
    [SerializeField] private bool applySmoothing = true; //Smoothly change light value or not
    [SerializeField] private float lightResponsiveness = 10f; //Time value for lerping to target light value
    public float lightTotal; //Amount of light hitting player, from 0 to 1

    [Min(0.25f)]
    public float lightDetectionCooldown;

    public void Start()
    {
        StartCoroutine(CheckLightValue());
    }

    public IEnumerator CheckLightValue() 
    {
        yield return new WaitForSeconds(lightDetectionCooldown);
        if (!(LightManager.Instance.Lights == null || LightManager.Instance.Lights.Length == 0)) { 

            targetLightTotal = 0; //Reset light total

            foreach (Light light in LightManager.Instance.Lights)
            {

                if (!light.gameObject.activeInHierarchy || //Check if light is actually active and exists
                    light.enabled == false ||
                    light == null) continue;

                //Select type of light
                switch (light.type)
                {
                    case LightType.Directional:
                        targetLightTotal += CheckDirectionalLight(light);
                        break;

                    case LightType.Point:
                        targetLightTotal += CheckPointLight(light);
                        break;

                    case LightType.Spot:
                        targetLightTotal += CheckSpotLight(light);
                        break;

                    default:
                        Debug.Log("Unknown Light Type");
                        break;
                }
            }

            lightTotal = applySmoothing ? Mathf.Lerp(lightTotal, targetLightTotal, Time.deltaTime * lightResponsiveness) : targetLightTotal; //Smoothly interpolate if smoothing is applied

            lightTotal = Mathf.Clamp01(lightTotal); //Keep value within 0 to 1
            Debug.Log($"{gameObject}'s light value is {lightTotal}");
        }
        StartCoroutine(CheckLightValue());
    }

    #region LightCheck
    private float CheckDirectionalLight(Light _directionalLight)
    {
        //Get the opposite direction the light is going in and use that to shoot a ray from the player. If it's blocked, then light is not reaching the player
        Vector3 lightDirectionReverse = _directionalLight.transform.forward * -1;

        if (Physics.Raycast(transform.position, lightDirectionReverse, float.MaxValue, lightHitMask))
        {
            //Directional Light is blocked by something
            return 0;
        }
        else
        {
            //Directional Light isn't blocked
            return Mathf.Clamp01(_directionalLight.intensity);
        }

    }

    private float CheckPointLight(Light _pointLight)
    {
        //Check if player is within range of the light
        float distance = Vector3.Distance(transform.position, _pointLight.transform.position);
        if (distance < _pointLight.range)
        {
            //Do a raycast check to see if the light is acutally reaching the player
            Vector3 direction = transform.position - _pointLight.transform.position;

            if (Physics.Raycast(_pointLight.transform.position, direction, out lightHitInfo, float.MaxValue, lightHitMask)) //Fire ray from light to attached gameobject to make sure the attached gameobject collider is hit
            {
                if (lightHitInfo.transform.tag == gameObject.tag)
                {
                    //Point light is reaching player
                    return Mathf.Clamp01(_pointLight.intensity * (1 / direction.sqrMagnitude));
                }
            }
        }
        return 0;
    }

    private float CheckSpotLight(Light _spotLight)
    {

        Vector3 direction = transform.position - _spotLight.transform.position;

        float distance = direction.magnitude;
        float angle = Vector3.Angle(_spotLight.transform.forward, direction);

        //Check player in light range and angle btwn light direction and direction to player is within cone by dividing by 2
        //(Note: A 60 degree cone would need to check within 30 degress of the forward vector of the light)

        if (distance < _spotLight.range && angle < _spotLight.spotAngle / 2)
        {

            if (Physics.Raycast(_spotLight.transform.position, direction, out lightHitInfo, float.MaxValue, lightHitMask))
            {
                if (lightHitInfo.transform.tag == gameObject.tag)
                {
                    //Spot light is reaching player
                    return Mathf.Clamp01(_spotLight.intensity * (1 / direction.sqrMagnitude));
                }
            }
        }
        return 0;
    }
    #endregion

}