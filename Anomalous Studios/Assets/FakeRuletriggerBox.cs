using UnityEngine;

public class FakeRuletriggerBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            // Play the sound
        }
    }
}
