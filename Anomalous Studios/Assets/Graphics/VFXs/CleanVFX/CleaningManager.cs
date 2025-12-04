using UnityEngine;

public class CleaningManager : MonoBehaviour
{
    [Header("Camera & Settings")]
    public Camera playerCamera;          // Player camera
    public float cleanDistance = 3f;     // Max ray distance
    public KeyCode cleanKey = KeyCode.L; // Toggle key

    [Header("Progress Settings")]
    [Tooltip("How often to check the cleaning progress (seconds).")]
    public float progressCheckInterval = 1f;

    [Tooltip("The clean threshold (0–1). When exceeded, OnSurfaceCleaned() is triggered.")]
    [Range(0f, 1f)] public float cleanThreshold = 0.8f;

    private bool isCleaning = false;     // Toggle state
    private float lastCheckTime = 0f;    // Timer for progress check

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        // Toggle cleaning mode with key
        //if (Input.GetKeyDown(cleanKey))
        //{
        //    isCleaning = !isCleaning;
        //    Debug.Log(isCleaning ? " Cleaning mode ON" : "Cleaning mode OFF");
        //}

        // Perform cleaning when active
        if (isCleaning)
        {
            TryClean();
        }

      

        // Periodically check progress
        if (Time.time - lastCheckTime > progressCheckInterval)
        {
            lastCheckTime = Time.time;
            CheckCleaningProgress();
        }
    }

    /// <summary>
    /// Toggles between cleaning mode (for mop item)
    /// </summary>
    public void ToggleCleaningMode()
    {
        isCleaning = !isCleaning;
    }

    /// <summary>
    /// Fires a ray from the camera and cleans if it hits a SurfaceCleaner.
    /// </summary>
    void TryClean()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, cleanDistance))
        {
            SurfaceCleaner cleaner = hit.collider.GetComponent<SurfaceCleaner>();
            if (cleaner != null)
            {
                cleaner.CleanAtUV(hit.textureCoord);
            }
        }
    }

    /// <summary>
    /// Checks how much of the surface is clean.
    /// </summary>
    void CheckCleaningProgress()
    {
        SurfaceCleaner[] cleaners =  FindObjectsOfType<SurfaceCleaner>(); // Object.FindObjectsByType<SurfaceCleaner>(FindObjectsSortMode.None);
        if (cleaners.Length == 0) return;

        foreach (var cleaner in cleaners)
        {
            float progress = cleaner.GetCleanProgress();

            if (progress >= cleanThreshold)
            {
                OnSurfaceCleaned(cleaner);
            }
        }
    }

    /// <summary>
    /// Triggered when a surface reaches or exceeds the clean threshold.
    /// </summary>
    void OnSurfaceCleaned(SurfaceCleaner cleaner)
    {
        Debug.Log($"{cleaner.name} is clean enough! (> {cleanThreshold * 100f:F0}%)");
        GameVariables.Set("floor-cleaned", "true");
    }
}
