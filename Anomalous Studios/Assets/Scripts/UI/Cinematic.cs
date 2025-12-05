using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cinematic : MonoBehaviour
{
    GameObject player;
    [SerializeField] GameObject sceneLoader;
    [SerializeField] TMP_Text skipText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Turns off player for cutscene
        if(GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            player.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            skipText.gameObject.SetActive(true);
        }
        if (skipText.gameObject.activeSelf) 
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                sceneLoader.SetActive(true);
            }
        }
    }
}
