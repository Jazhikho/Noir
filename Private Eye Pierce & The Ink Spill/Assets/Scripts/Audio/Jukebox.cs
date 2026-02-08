using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Randomly plays one track from a list; when it ends, picks and plays another. Add to a GameObject with (or without) an AudioSource.
/// Supports FadeOut for transitions (e.g. quit to main menu).
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Jukebox : MonoBehaviour
{
    [Header("Tracks")]
    [Tooltip("List of music clips to play in random order. Leave empty to disable.")]
    public List<AudioClip> tracks = new List<AudioClip>();

    [Header("Playback")]
    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Tooltip("Seconds of silence between tracks. 0 = start next immediately.")]
    public float gapBetweenTracks = 0f;
    [Tooltip("If true, never play the same track twice in a row.")]
    public bool avoidRepeat = true;

    private AudioSource _audioSource;
    private AudioClip _lastPlayedClip;
    private float _gapTimer;
    private bool _inGap;
    private bool _isFading;
    private Coroutine _fadeCoroutine;

    /// <summary>
    /// Caches or adds AudioSource and starts playback if tracks exist.
    /// </summary>
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();

        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
        _audioSource.volume = volume;
    }

    private void Start()
    {
        if (tracks != null && tracks.Count > 0)
            PlayNext();
    }

    private void Update()
    {
        if (_isFading) return;
        if (tracks == null || tracks.Count == 0) return;

        if (_inGap)
        {
            _gapTimer -= Time.deltaTime;
            if (_gapTimer <= 0f)
            {
                _inGap = false;
                PlayNext();
            }
            return;
        }

        if (_audioSource.isPlaying) return;

        if (gapBetweenTracks > 0f)
        {
            _inGap = true;
            _gapTimer = gapBetweenTracks;
        }
        else
        {
            PlayNext();
        }
    }

    /// <summary>
    /// Picks a random track (optionally different from the last) and plays it.
    /// </summary>
    public void PlayNext()
    {
        if (tracks == null || tracks.Count == 0) return;

        AudioClip next = PickRandomTrack();
        if (next == null) return;

        _lastPlayedClip = next;
        _audioSource.clip = next;
        _audioSource.volume = volume;
        _audioSource.Play();
    }

    /// <summary>
    /// Stops playback and clears the last-played state.
    /// </summary>
    public void Stop()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }
        _isFading = false;
        _inGap = false;
        _audioSource.Stop();
        _lastPlayedClip = null;
    }

    /// <summary>
    /// Fades volume to zero over the given duration, then stops. Use when transitioning to main menu.
    /// </summary>
    /// <param name="duration">Seconds over which to fade out.</param>
    public void FadeOut(float duration)
    {
        if (duration <= 0f)
        {
            Stop();
            return;
        }
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        _isFading = true;
        float startVolume = _audioSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            _audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        _audioSource.volume = 0f;
        _audioSource.Stop();
        _lastPlayedClip = null;
        _inGap = false;
        _isFading = false;
        _fadeCoroutine = null;
    }

    /// <summary>
    /// Returns true if the jukebox has tracks and is either playing or in a gap.
    /// </summary>
    public bool IsActive()
    {
        if (tracks == null || tracks.Count == 0) return false;
        return _audioSource.isPlaying || _inGap;
    }

    private AudioClip PickRandomTrack()
    {
        if (tracks == null || tracks.Count == 0) return null;

        List<AudioClip> candidates = new List<AudioClip>(tracks);
        if (avoidRepeat && candidates.Count > 1 && _lastPlayedClip != null)
            candidates.RemoveAll(c => c == _lastPlayedClip);

        if (candidates.Count == 0) return null;

        int index = Random.Range(0, candidates.Count);
        return candidates[index];
    }
}
