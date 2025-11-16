using AudioSystem;
using Unity.Behavior;
using UnityEngine;

enum TestValues
{
    RuleBreak,
    TaskComplete,
    MakeNoise,
    LoadLevel
}

/// <summary>
/// Test class to break the Rulekeeper's rules
/// </summary>
public class RageTotem : MonoBehaviour, IInteractable
{
    [SerializeField] private float _holdTime = 0.0f;
    [SerializeField] private TestValues _value;

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

    public void Highlight()
    {
        if (_canInteract) { GetComponent<AutoOutline>().IsHighlighted = true; }
    }
    public void RemoveHighlight()
    {
        if (_canInteract) { GetComponent<AutoOutline>().IsHighlighted = false; }
    }

    public void Interact()
    {
        switch(_value)
        {
            case TestValues.RuleBreak:
                EventBus<RuleBroken>.Raise(new RuleBroken { isBroken = true, target = transform.position });
                break;
            case TestValues.TaskComplete:
                EventBus<TaskComplete>.Raise(new TaskComplete { });
                break;
            case TestValues.LoadLevel:
                EventBus<LoadLevel>.Raise(new LoadLevel { newLevel = (Level)SceneLoader.CurrentLevel });
                break;
            case TestValues.MakeNoise:
                EventBus<MakeNoise>.Raise(new MakeNoise { target = transform.position });
                break;
        }
    }

}
