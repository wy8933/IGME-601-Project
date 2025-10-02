using TMPro;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fpsText; // Assign your UI Text element in the Inspector
    private float _refreshRate = 1f; // How often to update the display
    private float _timer;


    // Update is called once per frame
    void Update()
    {
        if (Time.unscaledTime > _timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            fpsText.text = "FPS: " + fps;
            _timer = Time.unscaledTime + _refreshRate;
        }
    }
}
