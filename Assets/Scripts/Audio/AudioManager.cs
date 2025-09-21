using UnityEngine;
using System.Collections.Generic;
using NeuralRink.Systems;

namespace NeuralRink.Audio
{
    /// <summary>
    /// Centralized audio management system for Neural Rink.
    /// Handles all game audio including sound effects, music, and ambient sounds.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource ambientSource;
        
        [Header("Sound Effects")]
        [SerializeField] private AudioClip goalSound;
        [SerializeField] private AudioClip saveSound;
        [SerializeField] private AudioClip puckHitSound;
        [SerializeField] private AudioClip stickHitSound;
        [SerializeField] private AudioClip skateSound;
        [SerializeField] private AudioClip crowdCheerSound;
        [SerializeField] private AudioClip crowdBooSound;
        
        [Header("Music")]
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private AudioClip trainingMusic;
        
        [Header("Ambient Sounds")]
        [SerializeField] private AudioClip rinkAmbient;
        [SerializeField] private AudioClip crowdAmbient;
        
        [Header("Audio Settings")]
        [SerializeField] private float masterVolume = 1f;
        [SerializeField] private float musicVolume = 0.7f;
        [SerializeField] private float sfxVolume = 1f;
        [SerializeField] private float ambientVolume = 0.5f;
        
        [Header("Performance")]
        [SerializeField] private bool disableInTraining = true;
        [SerializeField] private TrainingSwitch trainingSwitch;
        
        // Audio pools for performance
        private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
        private List<AudioSource> activeAudioSources = new List<AudioSource>();
        
        // Singleton instance
        public static AudioManager Instance { get; private set; }
        
        /// <summary>
        /// Initialize audio manager.
        /// </summary>
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioManager();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Initialize audio manager components.
        /// </summary>
        private void InitializeAudioManager()
        {
            // Find training switch
            if (trainingSwitch == null)
            {
                trainingSwitch = FindObjectOfType<TrainingSwitch>();
            }
            
            // Create audio sources if not assigned
            CreateAudioSources();
            
            // Initialize audio source pool
            InitializeAudioSourcePool();
            
            // Setup initial audio
            SetupInitialAudio();
        }
        
        /// <summary>
        /// Create audio sources if not assigned.
        /// </summary>
        private void CreateAudioSources()
        {
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }
            
            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
            
            if (ambientSource == null)
            {
                GameObject ambientObj = new GameObject("AmbientSource");
                ambientObj.transform.SetParent(transform);
                ambientSource = ambientObj.AddComponent<AudioSource>();
                ambientSource.loop = true;
                ambientSource.playOnAwake = false;
            }
        }
        
        /// <summary>
        /// Initialize audio source pool for performance.
        /// </summary>
        private void InitializeAudioSourcePool()
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject poolObj = new GameObject($"PooledAudioSource_{i}");
                poolObj.transform.SetParent(transform);
                AudioSource audioSource = poolObj.AddComponent<AudioSource>();
                audioSource.loop = false;
                audioSource.playOnAwake = false;
                audioSourcePool.Enqueue(audioSource);
            }
        }
        
        /// <summary>
        /// Setup initial audio based on training mode.
        /// </summary>
        private void SetupInitialAudio()
        {
            if (trainingSwitch != null && trainingSwitch.IsTrainingMode() && disableInTraining)
            {
                // Disable audio in training mode
                SetMasterVolume(0f);
                return;
            }
            
            // Start background music
            if (backgroundMusic != null)
            {
                PlayBackgroundMusic();
            }
            
            // Start ambient sounds
            if (rinkAmbient != null)
            {
                PlayAmbientSound(rinkAmbient);
            }
        }
        
        /// <summary>
        /// Play background music.
        /// </summary>
        public void PlayBackgroundMusic()
        {
            if (musicSource == null || backgroundMusic == null) return;
            
            musicSource.clip = backgroundMusic;
            musicSource.volume = musicVolume * masterVolume;
            musicSource.Play();
        }
        
        /// <summary>
        /// Play training music.
        /// </summary>
        public void PlayTrainingMusic()
        {
            if (musicSource == null || trainingMusic == null) return;
            
            musicSource.clip = trainingMusic;
            musicSource.volume = musicVolume * masterVolume;
            musicSource.Play();
        }
        
        /// <summary>
        /// Stop background music.
        /// </summary>
        public void StopBackgroundMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }
        
        /// <summary>
        /// Play ambient sound.
        /// </summary>
        public void PlayAmbientSound(AudioClip clip)
        {
            if (ambientSource == null || clip == null) return;
            
            ambientSource.clip = clip;
            ambientSource.volume = ambientVolume * masterVolume;
            ambientSource.Play();
        }
        
        /// <summary>
        /// Play sound effect.
        /// </summary>
        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            
            AudioSource source = GetPooledAudioSource();
            if (source != null)
            {
                source.clip = clip;
                source.volume = volume * sfxVolume * masterVolume;
                source.Play();
                
                // Return to pool when finished
                StartCoroutine(ReturnAudioSourceToPool(source, clip.length));
            }
        }
        
        /// <summary>
        /// Play sound effect at specific position.
        /// </summary>
        public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null) return;
            
            AudioSource source = GetPooledAudioSource();
            if (source != null)
            {
                source.transform.position = position;
                source.clip = clip;
                source.volume = volume * sfxVolume * masterVolume;
                source.spatialBlend = 1f; // 3D sound
                source.Play();
                
                // Return to pool when finished
                StartCoroutine(ReturnAudioSourceToPool(source, clip.length));
            }
        }
        
        /// <summary>
        /// Play goal sound effect.
        /// </summary>
        public void PlayGoalSound()
        {
            PlaySFX(goalSound);
            PlaySFX(crowdCheerSound, 0.8f);
        }
        
        /// <summary>
        /// Play save sound effect.
        /// </summary>
        public void PlaySaveSound()
        {
            PlaySFX(saveSound);
            PlaySFX(crowdCheerSound, 0.6f);
        }
        
        /// <summary>
        /// Play puck hit sound effect.
        /// </summary>
        public void PlayPuckHitSound(Vector3 position, float intensity = 1f)
        {
            PlaySFXAtPosition(puckHitSound, position, intensity);
        }
        
        /// <summary>
        /// Play stick hit sound effect.
        /// </summary>
        public void PlayStickHitSound(Vector3 position, float intensity = 1f)
        {
            PlaySFXAtPosition(stickHitSound, position, intensity);
        }
        
        /// <summary>
        /// Play skate sound effect.
        /// </summary>
        public void PlaySkateSound(Vector3 position, float intensity = 1f)
        {
            PlaySFXAtPosition(skateSound, position, intensity * 0.5f);
        }
        
        /// <summary>
        /// Play crowd boo sound effect.
        /// </summary>
        public void PlayCrowdBooSound()
        {
            PlaySFX(crowdBooSound, 0.7f);
        }
        
        /// <summary>
        /// Get pooled audio source.
        /// </summary>
        private AudioSource GetPooledAudioSource()
        {
            if (audioSourcePool.Count > 0)
            {
                AudioSource source = audioSourcePool.Dequeue();
                activeAudioSources.Add(source);
                return source;
            }
            
            // Create new audio source if pool is empty
            GameObject newObj = new GameObject("TempAudioSource");
            newObj.transform.SetParent(transform);
            AudioSource newSource = newObj.AddComponent<AudioSource>();
            newSource.loop = false;
            newSource.playOnAwake = false;
            activeAudioSources.Add(newSource);
            
            return newSource;
        }
        
        /// <summary>
        /// Return audio source to pool.
        /// </summary>
        private System.Collections.IEnumerator ReturnAudioSourceToPool(AudioSource source, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (source != null)
            {
                source.Stop();
                source.clip = null;
                source.spatialBlend = 0f; // Reset to 2D
                
                activeAudioSources.Remove(source);
                audioSourcePool.Enqueue(source);
            }
        }
        
        /// <summary>
        /// Set master volume.
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();
        }
        
        /// <summary>
        /// Set music volume.
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();
        }
        
        /// <summary>
        /// Set SFX volume.
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }
        
        /// <summary>
        /// Set ambient volume.
        /// </summary>
        public void SetAmbientVolume(float volume)
        {
            ambientVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();
        }
        
        /// <summary>
        /// Update all audio source volumes.
        /// </summary>
        private void UpdateAllVolumes()
        {
            if (musicSource != null)
            {
                musicSource.volume = musicVolume * masterVolume;
            }
            
            if (ambientSource != null)
            {
                ambientSource.volume = ambientVolume * masterVolume;
            }
        }
        
        /// <summary>
        /// Mute all audio.
        /// </summary>
        public void MuteAll()
        {
            SetMasterVolume(0f);
        }
        
        /// <summary>
        /// Unmute all audio.
        /// </summary>
        public void UnmuteAll()
        {
            SetMasterVolume(1f);
        }
        
        /// <summary>
        /// Check if audio is muted.
        /// </summary>
        public bool IsMuted()
        {
            return masterVolume <= 0f;
        }
        
        /// <summary>
        /// Get current master volume.
        /// </summary>
        public float GetMasterVolume()
        {
            return masterVolume;
        }
        
        /// <summary>
        /// Get current music volume.
        /// </summary>
        public float GetMusicVolume()
        {
            return musicVolume;
        }
        
        /// <summary>
        /// Get current SFX volume.
        /// </summary>
        public float GetSFXVolume()
        {
            return sfxVolume;
        }
        
        /// <summary>
        /// Get current ambient volume.
        /// </summary>
        public float GetAmbientVolume()
        {
            return ambientVolume;
        }
        
        /// <summary>
        /// Stop all audio.
        /// </summary>
        public void StopAllAudio()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
            
            if (ambientSource != null)
            {
                ambientSource.Stop();
            }
            
            // Stop all active audio sources
            foreach (AudioSource source in activeAudioSources)
            {
                if (source != null)
                {
                    source.Stop();
                }
            }
        }
        
        /// <summary>
        /// Pause all audio.
        /// </summary>
        public void PauseAllAudio()
        {
            if (musicSource != null)
            {
                musicSource.Pause();
            }
            
            if (ambientSource != null)
            {
                ambientSource.Pause();
            }
            
            // Pause all active audio sources
            foreach (AudioSource source in activeAudioSources)
            {
                if (source != null)
                {
                    source.Pause();
                }
            }
        }
        
        /// <summary>
        /// Resume all audio.
        /// </summary>
        public void ResumeAllAudio()
        {
            if (musicSource != null)
            {
                musicSource.UnPause();
            }
            
            if (ambientSource != null)
            {
                ambientSource.UnPause();
            }
            
            // Resume all active audio sources
            foreach (AudioSource source in activeAudioSources)
            {
                if (source != null)
                {
                    source.UnPause();
                }
            }
        }
        
        /// <summary>
        /// Handle training mode changes.
        /// </summary>
        private void Update()
        {
            if (trainingSwitch != null && disableInTraining)
            {
                bool shouldMute = trainingSwitch.IsTrainingMode();
                if (shouldMute && !IsMuted())
                {
                    MuteAll();
                }
                else if (!shouldMute && IsMuted())
                {
                    UnmuteAll();
                }
            }
        }
        
        /// <summary>
        /// Cleanup on destroy.
        /// </summary>
        private void OnDestroy()
        {
            StopAllAudio();
        }
    }
}
