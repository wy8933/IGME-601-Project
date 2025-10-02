using UnityEngine;

public class DoorController : MonoBehaviour
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

    void Start()
    {
        closedRot = transform.rotation;
        openRot = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));

        // Default ease in/out curve is assigned here
        if (easeCurve == null || easeCurve.length == 0)
            easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        // Find player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        // Check if player is in range
        float dist = Vector3.Distance(player.position, transform.position);

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
}
