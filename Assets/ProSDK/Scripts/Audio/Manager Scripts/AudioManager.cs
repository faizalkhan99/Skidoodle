using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// --- ONE-SHOT SOUND DEFINITIONS ---
public enum OneShotSoundID { ButtonClick, LevelComplete, GameWin, GameOver, BalloonPop, Yay }
[System.Serializable] public class OneShotSound { public OneShotSoundID id; public AudioClip clip; }

// --- LOOPING SOUND DEFINITIONS ---
public enum LoopingSoundID { CarDrift, CarEngine }

// NEW: This enum in the data tells the generic function which source to use.
public enum LoopType { Engine, Effect }

[System.Serializable]
public class LoopingSound
{
    public LoopingSoundID id;
    public AudioClip clip;
    public LoopType type; // The data that makes our generic system smart.
}


public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource[] _sfxSources;
    [Tooltip("The dedicated source for primary looping sounds like engines.")]
    [SerializeField] private AudioSource _engineSource;
    [Tooltip("The dedicated source for secondary looping effects like drifting.")]
    [SerializeField] private AudioSource _effectSource;

    [Header("Pitch Control")]
    [SerializeField, Range(0.5f, 2f)] private float _minEnginePitch = 0.8f;
    [SerializeField, Range(1f, 3f)] private float _maxEnginePitch = 2.0f;

    [Header("Libraries")]
    [SerializeField] private OneShotSound[] _oneShotSfxLibrary;
    [SerializeField] private LoopingSound[] _loopingSfxLibrary;

    private Dictionary<OneShotSoundID, AudioClip> _oneShotSfxDictionary;
    private Dictionary<LoopingSoundID, LoopingSound> _loopingSfxDictionary; // Now stores the whole class

    private void Awake()
    {
        // Populate Dictionaries
        _oneShotSfxDictionary = new Dictionary<OneShotSoundID, AudioClip>();
        foreach (var sound in _oneShotSfxLibrary) _oneShotSfxDictionary[sound.id] = sound.clip;

        _loopingSfxDictionary = new Dictionary<LoopingSoundID, LoopingSound>();
        foreach (var sound in _loopingSfxLibrary) _loopingSfxDictionary[sound.id] = sound;
    }

    // --- GENERIC LOOPING METHODS ---

    public void StartLoopingSFX(LoopingSoundID id)
    {
        if (!_loopingSfxDictionary.TryGetValue(id, out LoopingSound soundToPlay)) return;

        // The data determines which source to use
        AudioSource source = (soundToPlay.type == LoopType.Engine) ? _engineSource : _effectSource;

        if (source == null) return;

        source.clip = soundToPlay.clip;
        source.loop = true;
        source.Play();
    }

    public void StopLoopingSFX(LoopingSoundID id)
    {
        if (!_loopingSfxDictionary.TryGetValue(id, out LoopingSound soundToPlay)) return;

        AudioSource source = (soundToPlay.type == LoopType.Engine) ? _engineSource : _effectSource;

        if (source != null && source.isPlaying && source.clip == soundToPlay.clip)
        {
            source.Stop();
        }
    }

    // This is the only semi-specific method, as pitch is tied to the engine source.
    public void UpdateEnginePitch(float normalizedRPM)
    {
        if (_engineSource != null && _engineSource.isPlaying)
        {
            _engineSource.pitch = Mathf.Lerp(_minEnginePitch, _maxEnginePitch, normalizedRPM);
        }
    }
    // --- BGM and One-Shot methods ---
    public void OnChangeBGM(AudioClip newClip)
    {
        // This log tells us every time the method is called.
        Debug.Log($"<color=yellow>[AudioManager.OnChangeBGM] Received a request.</color>");

        // This log tells us EXACTLY what clip was received.
        if (newClip != null)
        {
            Debug.Log($"<color=yellow>  - Received Clip:</color> {newClip.name}");
        }
        else
        {
            Debug.Log($"<color=orange>  - Received Clip: NULL</color>");
        }

        // Don't restart the music if the same track is requested again.
        if (_bgmSource.clip == newClip)
        {
            Debug.Log("<color=yellow>  - Clip is the same as current. No action taken.</color>");
            return;
        }

        _bgmSource.Stop();
        _bgmSource.clip = newClip;

        if (newClip != null)
        {
            _bgmSource.Play();
            Debug.Log("<color=yellow>  - Action: Playing new clip.</color>");
        }
        else
        {
            Debug.Log("<color=yellow>  - Action: Clip was null, music stopped.</color>");
        }
    }
    public void OnPauseBGM() => _bgmSource?.Pause();
    public void OnResumeBGM() => _bgmSource?.UnPause();

    // --- GENERIC ONE-SHOT METHOD ---
    public void PlayOneShotSFX(OneShotSoundID id)
    {
        if (!_oneShotSfxDictionary.TryGetValue(id, out AudioClip clipToPlay))
        {
            Debug.LogWarning("AudioManager: One-shot Sound ID not found in library: " + id);
            return;
        }

        // Find an available source from the pool to play the sound.
        for (int i = 0; i < _sfxSources.Length; i++)
        {
            if (_sfxSources[i] != null && !_sfxSources[i].isPlaying)
            {
                _sfxSources[i].PlayOneShot(clipToPlay);
                return;
            }
        }

        // If all pooled sources are busy, create a temporary one as a fallback.
        StartCoroutine(CreateTemporarySourceAndPlay(clipToPlay));
    }

    private IEnumerator CreateTemporarySourceAndPlay(AudioClip clip)
    {
        GameObject tempGO = new GameObject("TempAudio_" + clip.name);
        tempGO.transform.SetParent(this.transform);
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();

        if (_sfxSources.Length > 0 && _sfxSources[0] != null)
        {
            tempSource.outputAudioMixerGroup = _sfxSources[0].outputAudioMixerGroup;
            tempSource.spatialBlend = _sfxSources[0].spatialBlend;
        }

        tempSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        Destroy(tempGO);
    }
}