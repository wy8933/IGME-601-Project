using UnityEngine;

public class PlayerJournal : MonoBehaviour
{
    private bool _inJournal = false;
    [Header("Handbook")]
    //[SerializeField] private Handbook_UI handbook; <-- WILL BE REMOVED
    private Handbook_UI handbook;

    private EventBinding<LevelLoaded> _levelLoaded;

    // Getter Methods
    public bool GetInJournal() { return _inJournal; }

    private void Start()
    {
        // Transform.Find() searches for the handbook_UI even if its disabled
        handbook = GameObject.Find("MainUI").transform.Find("Handbook").GetComponent<Handbook_UI>();
    }

    public void ToggleHandbook()
    {
        _inJournal = !_inJournal;
        handbook.gameObject.SetActive(_inJournal);
        if (_inJournal)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
    }
}
