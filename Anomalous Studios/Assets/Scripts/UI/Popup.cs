using System.Collections;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] CanvasGroup _canvasGroup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Destroy any previous popups that may be active.
        GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("Popup");
        foreach (GameObject obj in objectsToDestroy) 
        {
            // Ignore this game object
            if (obj == this.gameObject) 
            {
                StartCoroutine(DoFade(1, 0, 3));
            }
            else
            {
                Destroy(obj);
            }
        
        }
        
    }

    /// <summary>
    /// Coroutine to fade out the canvas goup over a custom duration
    /// </summary>
    /// <param name="startAlpha"></param>
    /// <param name="endAlpha"></param>
    /// <param name="_fadeDuration"></param>
    /// <returns></returns>
    private IEnumerator DoFade(float startAlpha, float endAlpha, float _fadeDuration)
    {
        float timer = 0;

        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / _fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = endAlpha;
        Destroy(gameObject);
    }
}
