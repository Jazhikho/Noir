using UnityEngine;
using System.Collections;

public class FootstepLoop : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip footstepClip;
    [Range(0f, 1f)]
    public float volume = 0.5f;

    [Header("References")]
    public PointClickController playerController;

    [Header("Settings")]
    public float stopDelay = 0.1f;
    public bool footstepsEnabled = true;

    private bool isPlaying = false;
    private Coroutine stopCoroutine;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = footstepClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = volume;

        if (playerController == null)
            playerController = GetComponent<PointClickController>();

        if (playerController == null)
            playerController = FindFirstObjectByType<PointClickController>();
    }

    void Update()
    {
        if (!footstepsEnabled || playerController == null) return;

        bool shouldPlay = playerController.IsMoving() && playerController.IsMovementEnabled();

        if (shouldPlay && !isPlaying)
            StartFootsteps();
        else if (!shouldPlay && isPlaying)
            StopFootstepsDelayed();
    }

    void StartFootsteps()
    {
        if (stopCoroutine != null)
        {
            StopCoroutine(stopCoroutine);
            stopCoroutine = null;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.volume = volume;
            audioSource.Play();
        }

        isPlaying = true;
    }

    void StopFootstepsDelayed()
    {
        if (stopCoroutine != null) return;

        stopCoroutine = StartCoroutine(StopAfterDelay());
    }

    IEnumerator StopAfterDelay()
    {
        yield return new WaitForSeconds(stopDelay);

        if (playerController == null || !playerController.IsMoving())
        {
            audioSource.Stop();
            isPlaying = false;
        }

        stopCoroutine = null;
    }

    public void SetEnabled(bool enabled)
    {
        footstepsEnabled = enabled;

        if (!enabled)
            ForceStop();
    }

    public void ForceStop()
    {
        if (stopCoroutine != null)
        {
            StopCoroutine(stopCoroutine);
            stopCoroutine = null;
        }

        if (audioSource != null)
            audioSource.Stop();

        isPlaying = false;
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);

        if (audioSource != null)
            audioSource.volume = volume;
    }
}
