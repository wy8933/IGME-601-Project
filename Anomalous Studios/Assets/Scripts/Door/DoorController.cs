using UnityEngine;
using ItemSystem;
using AudioSystem;

public class DoorController : ItemInstance
{
    [Header("Parameters")]
    public float openAngle = 110f;       // Door Angle
    public float duration = 1f;       // Anim Time
    public AnimationCurve easeCurve;    // Ease in/out
    public float interactRange = 3f;    // Player interact distance
    public string playerTag = "Player"; // Player Tag
    public string DoorID = "101";

    private bool isOpen = false;        // Door state
    private bool isAnimating = false;   // Anim flag

    private Quaternion closedRot;   // Close Angle
    private Quaternion openRot;     // Open Angle
    private Quaternion startRot;    // Starting Angle
    private Quaternion targetRot;   // Target Angle

    private float t = 0f;           // Anim Progress
    private Transform player;       // Reference to player
    private bool unlocked;          // Whether the door is locked/unlocked

    [Header("Reaction SFX")]
    [SerializeField] private SoundDataSO _failedSFX;
    [SerializeField] private SoundDataSO _successSFX;
    public override SoundDataSO InitialSFX => null;
    public override SoundDataSO FailedSFX { get => _failedSFX; }
    public override SoundDataSO CancelSFX => null;
    public override SoundDataSO SuccessSFX { get => _successSFX; }

    // Getter Methods
    public bool GetUnlocked() { return unlocked; }

    // Setter Methods
    public void SetUnlocked(bool val) { unlocked = val; }

    void Start()
    {
        Initialize();

        closedRot = transform.rotation;
        openRot = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));

        // Default ease in/out curve is assigned here
        if (easeCurve == null || easeCurve.length == 0)
            easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        // Find player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
            player = playerObj.transform;

        unlocked = false;
    }

    void Update()
    {
        // Animate rotation
        if (isAnimating)
        {
            t += Time.deltaTime / duration;
            float curveT = easeCurve.Evaluate(t); // Apply ease in/out
            transform.rotation = Quaternion.Slerp(startRot, targetRot, curveT);

            if (t >= 1f) // End of the anim
            {
                transform.rotation = targetRot;
                isAnimating = false;
            }
        }
    }

    public override void Use(GameObject user)
    {
        TryUse(user);
    }

    public void ToggleDoor()
    {
        if (!isAnimating)
        {
            isOpen = !isOpen;
            startRot = transform.rotation;
            targetRot = isOpen ? openRot : closedRot;

            t = 0f;
            isAnimating = true;
        }
    }

    // Debug visualization in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }

    public override void Interact()
    {
        Debug.Log("interacting with door");
        if (IInteractable.Instigator != null && unlocked)
        {
            ToggleDoor();
        }
    }
}
