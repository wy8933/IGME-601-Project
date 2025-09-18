using UnityEngine;

namespace AudioSystem
{
    /// <summary>
    /// Categories used to group different types of audio.
    /// </summary>
    public enum SoundCategory { SFX, Music, UI }

    /// <summary>
    /// ScriptableObject that stores data for playing sounds.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSoundData", menuName = "AudioSystem/Sound Data")]
    public class SoundDataSO : ScriptableObject
    {
        [Tooltip("The audio clip to play.")]
        public AudioClip Clip;

        [Tooltip("The category this sound belongs to (SFX, Music, or UI).")]
        public SoundCategory Category;

        [Tooltip("Whether the audio should loop when played.")]
        public bool Loop;

        [Tooltip("Whether this sound should be played in 3D space.")]
        public bool Is3D;

        [Tooltip("The base volume of the sound (0 = silent, 1 = full volume).")]
        [Range(0f, 1f)] public float Volume = 1f;
    }
}
