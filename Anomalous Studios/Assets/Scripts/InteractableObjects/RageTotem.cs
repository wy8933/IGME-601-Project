using AudioSystem;
using Unity.Behavior;
using UnityEngine;

/// <summary>
/// Test class to break the Rulekeeper's rules
/// </summary>
public class RageTotem : MonoBehaviour, IInteractable
{
    [SerializeField] private BehaviorGraphAgent _ruleKeeper;

    [SerializeField] private float _holdTime = 0.0f;

    private bool _canInteract = true;

    public float HoldTime { get => _holdTime; }
    public bool CanInteract { get => _canInteract; set => _canInteract = value; }

    [Header("Reaction SFX")]
    [SerializeField] private SoundDataSO _failedSFX;
    [SerializeField] private SoundDataSO _successSFX;
    public SoundDataSO InitialSFX => null;
    public SoundDataSO FailedSFX { get => _failedSFX; }
    public SoundDataSO CancelSFX => null;
    public SoundDataSO SuccessSFX { get => _successSFX; }

    public void Start()
    {
        _ruleKeeper = GameObject.FindGameObjectWithTag("RuleKeeper").GetComponent<BehaviorGraphAgent>();
    }

    public void Highlight()
    {
        // Highlight!
    }

    public void Interact()
    {
        _ruleKeeper.SetVariableValue("ruleBroken", true);
    }

    public void RemoveHighlight()
    {
        // Remove Highlight!
    }
}
