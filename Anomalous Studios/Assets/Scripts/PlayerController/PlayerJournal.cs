using UnityEngine;

public class PlayerJournal : MonoBehaviour
{
    private bool _inJournal = false;
    [Header("Handbook")]
    [SerializeField] private Handbook_UI handbook;

    // Getter Methods
    public bool GetInJournal() { return _inJournal; }

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
