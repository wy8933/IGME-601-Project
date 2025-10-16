using AudioSystem;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private SoundDataSO _floorChime;
    [SerializeField] private SoundDataSO _doorsMoving;

    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Waits for the close / open animation to fully finish before changing values
    /// </summary>
    public void IsOpen()
    {
        _animator.SetBool("isOpen", !_animator.GetBool("isOpen"));
    }

    public void OpenDoors()
    {
        //AudioManager.Instance.Play(_doorsMoving, transform.position);

        AudioManager.Instance.Play(_floorChime, transform.position);

        _animator.SetTrigger("moveDoors");
    }

    public void OnTriggerEnter(Collider other)
    {
        VariableConditionManager.Instance.Set("InElevator", "true");
    }

    public void OnTriggerExit(Collider other)
    {
        VariableConditionManager.Instance.Set("InElevator", "false");
    }
}
