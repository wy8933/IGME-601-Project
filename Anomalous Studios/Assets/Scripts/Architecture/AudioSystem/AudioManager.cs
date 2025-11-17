using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Audio;

namespace AudioSystem
{
    public class AudioManager : MonoBehaviour
    {

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

        [Header("Audio Source Instance Values")]
        [SerializeField] private GameObject _audioSourcePrefab;
        [SerializeField] private int _initialPoolSize = 10;
        [SerializeField] private int _maxPoolSize = 64;

        [Header("Audio Mixers")]
        public AudioMixer masterMixer;
        public AudioMixerGroup musicAudioMixerGroup;
        public AudioMixerGroup sfxAudioMixerGroup;
        public AudioMixerGroup uiAudioMixerGroup;


        /// <summary>
        /// Pool of available AudioSource objects.
        /// </summary>
        private readonly Queue<AudioSource> _audioSourcePool = new Queue<AudioSource>();

        /// <summary>
        /// Parent transform that holds pooled AudioSource objects.
        /// </summary>
        private readonly List<AudioSource> _allSources = new List<AudioSource>();
        private Transform _poolParent;

        private readonly HashSet<SoundCreator> _playingPairs = new HashSet<SoundCreator>();

        private struct SoundCreator
        {
            public readonly int emitterId;
            public readonly int clipId;
            public SoundCreator(GameObject emitter, AudioClip clip)
            {
                emitterId = emitter ? emitter.GetInstanceID() : 0;
                clipId = clip ? clip.GetInstanceID() : 0;
            }
        }

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
                EnqueueNewSource();
        }

        /// <summary>
        /// Plays a sound using pooled AudioSource with optional 3D positioning.
        /// </summary>
        /// <param name="soundData">Sound data to play.</param>
        /// <param name="position">Optional 3D world position.</param>
        public void Play(SoundDataSO soundData, Vector3? position = null)
        {
            Play(soundData, null, position);
        }

        public void Play(SoundDataSO soundData, GameObject creator, Vector3? position = null)
        {
            if (soundData == null || soundData.Clip == null) return;

            var key = new SoundCreator(creator, soundData.Clip);

            if (_playingPairs.Contains(key))
            {
                return;
            }

            var source = GetPooledAudioSource();
            if (source == null) return;

            switch (soundData.Category)
            {
                case SoundCategory.Music: source.outputAudioMixerGroup = musicAudioMixerGroup; break;
                case SoundCategory.SFX: source.outputAudioMixerGroup = sfxAudioMixerGroup; break;
                case SoundCategory.UI: source.outputAudioMixerGroup = uiAudioMixerGroup; break;
            }

            ConfigureSource(source, soundData, position);

            _playingPairs.Add(key);
            source.Play();

            if (!soundData.Loop)
                StartCoroutine(ReturnToPoolAfterPlayback(source, key));
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

        public void Stop(GameObject creator, SoundDataSO soundData)
        {
            if (soundData == null || soundData.Clip == null) return;
            var key = new SoundCreator(creator, soundData.Clip);

            for (int i = 0; i < _allSources.Count; i++)
            {
                var s = _allSources[i];
                if (s != null && s.clip == soundData.Clip && s.isPlaying)
                {
                    s.Stop();
                    s.clip = null;
                    s.gameObject.SetActive(false);
                    s.transform.SetParent(_poolParent);
                    _audioSourcePool.Enqueue(s);
                }
            }

            _playingPairs.Remove(key);
        }

        private void EnqueueNewSource()
        {
            var go = Instantiate(_audioSourcePrefab, _poolParent);
            go.SetActive(false);
            var src = go.GetComponent<AudioSource>();
            _audioSourcePool.Enqueue(src);
            _allSources.Add(src);
        }

        /// <summary>
        /// Retrieves an AudioSource from the pool or creates a new one if empty.
        /// </summary>
        /// <returns>Available AudioSource component.</returns>
        private AudioSource GetPooledAudioSource()
        {
            if (_audioSourcePool.Count > 0)
                return _audioSourcePool.Dequeue();

            if (_maxPoolSize <= 0 || _allSources.Count < _maxPoolSize)
            {
                EnqueueNewSource();
                return _audioSourcePool.Dequeue();
            }

            for (int i = 0; i < _allSources.Count; i++)
            {
                var s = _allSources[i];
                if (!s.isPlaying)
                {
                    s.gameObject.SetActive(false);
                    s.clip = null;
                    return s;
                }
            }

            return null;
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
        private IEnumerator ReturnToPoolAfterPlayback(AudioSource source, SoundCreator key)
        {
            yield return new WaitWhile(() => source.isPlaying);

            source.Stop();
            source.clip = null;
            source.gameObject.SetActive(false);
            source.transform.SetParent(_poolParent);
            _audioSourcePool.Enqueue(source);

            _playingPairs.Remove(key);
        }
    }
}
