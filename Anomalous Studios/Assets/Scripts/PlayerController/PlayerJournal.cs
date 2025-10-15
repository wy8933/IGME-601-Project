using UnityEngine;

public class PlayerJournal : MonoBehaviour
{
    private bool _inJournal = false;
    [Header("Handbook")]
    [SerializeField] private Handbook_UI handbook;

    private EventBinding<LevelLoaded> _levelLoaded;

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

    // TODO: can we deregister the event after the handbook has been initialized? No need to reset it every time
    private void InitReferences(LevelLoaded e)
    {
        handbook = e._handbook;
    }

    public void OnEnable()
    {
        _levelLoaded = new EventBinding<LevelLoaded>(InitReferences);
        EventBus<LevelLoaded>.Register(_levelLoaded);
    }

    public void OnDisable()
    {
        EventBus<LevelLoaded>.DeRegister(_levelLoaded);
    }
}
