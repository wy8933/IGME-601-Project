using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace AudioSystem
{
    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance of the AudioManager.
        /// </summary>
        public static AudioManager Instance { get; private set; }

        /// <summary>
        /// Stores volume levels for each sound category.
        /// </summary>
        private readonly Dictionary<SoundCategory, float> _categoryVolumes = new Dictionary<SoundCategory, float>
        {
            { SoundCategory.SFX, 1f },
            { SoundCategory.Music, 1f },
            { SoundCategory.UI, 1f }
        };

        [SerializeField] private GameObject _audioSourcePrefab;
        [SerializeField] private int _initialPoolSize = 10;

        /// <summary>
        /// Pool of available AudioSource objects.
        /// </summary>
        private readonly Queue<AudioSource> _audioSourcePool = new Queue<AudioSource>();

        /// <summary>
        /// Parent transform that holds pooled AudioSource objects.
        /// </summary>
        private Transform _poolParent;

        /// <summary>
        /// Initializes the singleton instance, audio source pool, and persists this manager across scenes.
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            _poolParent = new GameObject("AudioSourcePool").transform;
            DontDestroyOnLoad(_poolParent.gameObject);

            for (int i = 0; i < _initialPoolSize; i++)
                _audioSourcePool.Enqueue(CreateNewAudioSource());
        }

        /// <summary>
        /// Plays a sound using pooled AudioSource with optional 3D positioning.
        /// </summary>
        /// <param name="soundData">Sound data to play.</param>
        /// <param name="position">Optional 3D world position.</param>
        public void Play(SoundDataSO soundData, Vector3? position = null)
        {
            AudioSource source = GetPooledAudioSource();
            ConfigureSource(source, soundData, position);
            source.Play();

            if (!soundData.Loop)
                StartCoroutine(ReturnToPoolAfterPlayback(source));
        }

        /// <summary>
        /// Sets the volume for a specific sound category.
        /// </summary>
        /// <param name="category">Sound category.</param>
        /// <param name="volume">New volume (0 to 1).</param>
        public void SetCategoryVolume(SoundCategory category, float volume)
        {
            _categoryVolumes[category] = Mathf.Clamp01(volume);
        }

        /// <summary>
        /// Instantiates and returns a new AudioSource object from the prefab.
        /// </summary>
        /// <returns>New AudioSource component.</returns>
        private AudioSource CreateNewAudioSource()
        {
            GameObject go = Instantiate(_audioSourcePrefab, _poolParent);
            go.SetActive(false);
            return go.GetComponent<AudioSource>();
        }

        /// <summary>
        /// Retrieves an AudioSource from the pool or creates a new one if empty.
        /// </summary>
        /// <returns>Available AudioSource component.</returns>
        private AudioSource GetPooledAudioSource()
        {
            if (_audioSourcePool.Count > 0)
                return _audioSourcePool.Dequeue();
            return CreateNewAudioSource();
        }

        /// <summary>
        /// Configures the properties of an AudioSource based on sound data and position.
        /// </summary>
        /// <param name="source">AudioSource to configure.</param>
        /// <param name="soundData">Sound data settings.</param>
        /// <param name="position">Optional 3D position.</param>
        private void ConfigureSource(AudioSource source, SoundDataSO soundData, Vector3? position)
        {
            source.clip = soundData.Clip;
            source.loop = soundData.Loop;
            source.spatialBlend = soundData.Is3D ? 1f : 0f;
            source.volume = soundData.Volume * _categoryVolumes[soundData.Category];

            if (position.HasValue)
            {
                source.transform.position = position.Value;
                source.transform.parent = null;
            }
            else
            {
                source.transform.SetParent(_poolParent);
                source.transform.localPosition = Vector3.zero;
            }

            source.gameObject.SetActive(true);
        }

        /// <summary>
        /// Coroutine to return a non-looping AudioSource to the pool after it finishes playing.
        /// </summary>
        /// <param name="source">AudioSource that finished playing.</param>
        /// <returns>IEnumerator for coroutine.</returns>
        private IEnumerator ReturnToPoolAfterPlayback(AudioSource source)
        {
            yield return new WaitWhile(() => source.isPlaying);
            source.Stop();
            source.clip = null;
            source.gameObject.SetActive(false);
            source.transform.SetParent(_poolParent);
            _audioSourcePool.Enqueue(source);
        }
    }
}
