using UnityEngine;

namespace AudioSystem {
    public class AudioTester : MonoBehaviour
    {
        public SoundDataSO soundDataSO;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Debug.Log("Test Start");
            // Play audio in 2d
            AudioManager.Instance.Play(soundDataSO);

            // Play audio in 3d
            AudioManager.Instance.Play(soundDataSO, transform.position);

            Debug.Log(SoundCategory.Music);
            AudioManager.Instance.SetCategoryVolume(SoundCategory.Music, 0.3f);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) 
            {
                AudioManager.Instance.Play(soundDataSO);
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                AudioManager.Instance.Play(soundDataSO, transform.position);
            }
        }
    }
}