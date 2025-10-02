using System.Collections;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] CanvasGroup _canvasGroup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DoFade(1, 0, 5));
    }

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
