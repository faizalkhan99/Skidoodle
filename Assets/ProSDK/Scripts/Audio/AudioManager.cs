using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A modular, event-driven Audio Manager.
/// It has no singleton and is controlled by listening to events.
/// It has specific public methods for each sound, designed to be connected
/// to GameEventListeners in the Inspector.
/// </summary>
public class Molded_AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource[] _sfxSources; // Audio source pool

    [Header("Audio Clips Library")]
    [SerializeField] private Sound[] _sfxLibrary;

    private Dictionary<SoundID, AudioClip> _sfxDictionary;

    private void Awake()
    {
        // Populate the Dictionary for fast lookups.
        _sfxDictionary = new Dictionary<SoundID, AudioClip>();
        foreach (var sound in _sfxLibrary)
        {
            _sfxDictionary[sound.id] = sound.clip;
        }
    }

    private void Start()
    {
        // We no longer auto-play BGM. We wait for an event like "OnMainMenuLoaded".
        // For now, you can call this via a listener on a "GameStart" event.
        PlayBGM();
    }

    // --- BGM Controls (To be called by Event Listeners) ---
    public void PlayBGM() => _bgmSource?.PlayDelayed(0.3f);
    public void PauseBGM() => _bgmSource?.Pause();
    public void UnpauseBGM() => _bgmSource?.UnPause();

    // --- SFX Methods (To be called by Event Listeners) ---
    // Create one public method for each sound you want to trigger via an event.
    
    public void Play_ButtonClick() => PlaySFX(SoundID.ButtonClick);
    public void Play_LevelComplete() => PlaySFX(SoundID.LevelComplete);
    public void Play_GameWin() => PlaySFX(SoundID.GameWin);
    public void Play_GameOver() => PlaySFX(SoundID.GameOver);
    public void Play_BalloonPop() => PlaySFX(SoundID.BalloonPop);
    public void Play_Yay() => PlaySFX(SoundID.yay);
    // Add more methods here as needed for your other sounds...


    // --- Internal Sound Playing Logic ---
    private void PlaySFX(SoundID id)
    {
        if (!_sfxDictionary.TryGetValue(id, out AudioClip clipToPlay))
        {
            Debug.LogWarning("AudioManager: Sound ID not found in library: " + id);
            return;
        }
        
        // Find an available source from the pool
        for (int i = 0; i < _sfxSources.Length; i++)
        {
            if (!_sfxSources[i].isPlaying)
            {
                _sfxSources[i].PlayOneShot(clipToPlay);
                return;
            }
        }
        
        // If all pooled sources are busy, create a temporary one.
        StartCoroutine(CreateTemporarySourceAndPlay(clipToPlay));
    }

    private IEnumerator CreateTemporarySourceAndPlay(AudioClip clip)
    {
        GameObject tempGO = new GameObject("TempAudio_" + clip.name);
        tempGO.transform.SetParent(this.transform);
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();
        
        if (_sfxSources.Length > 0)
        {
            tempSource.outputAudioMixerGroup = _sfxSources[0].outputAudioMixerGroup;
            tempSource.spatialBlend = _sfxSources[0].spatialBlend;
        }

        tempSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        Destroy(tempGO);
    }
}
