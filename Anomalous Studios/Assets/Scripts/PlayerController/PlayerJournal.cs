using UnityEngine;

public class PlayerJournal : MonoBehaviour
{
    private bool _inJournal = false;
    [Header("Handbook")]
    //[SerializeField] private Handbook_UI handbook; <-- WILL BE REMOVED
    private Handbook_UI handbook;

    private EventBinding<LevelLoaded> _levelLoaded;

    // Reference variables needed to check conditions before enabling the elevator's open button
    private PlayerController _playerController;
    private ElevatorController _elevatorController;

    // Getter Methods
    public bool GetInJournal() { return _inJournal; }

    private void Start()
    {
        // Transform.Find() searches for the handbook_UI even if its disabled
        handbook = GameObject.Find("MainUI").transform.Find("Handbook").GetComponent<Handbook_UI>();

        // Find the references
        _playerController = this.GetComponentInParent<PlayerController>();
        _elevatorController = GameObject.FindGameObjectWithTag("Elevator").GetComponent<ElevatorController>();
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

        // Ensures that the player must view the handbook and have collected all policies in the elevator before elevator's open button is enabled
        if (_elevatorController.NotesCount() <= 0 && _playerController.ViewedHandbook) _elevatorController.GetOpenButton().Enable();
    }
}
