using UnityEngine;

public class StartGameLoader : MonoBehaviour
{
    [Tooltip("Where to go from the main menu, this should be the first level in the list")]
    [SerializeField] private Level _startLevel;
    GameObject player;
    private void OnEnable()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindGameObjectWithTag("Player").gameObject;
            player.SetActive(true);
        }
        EventBus<LoadLevel>.Raise(new LoadLevel { newLevel = _startLevel }); 
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
