using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class SearchableProp : MonoBehaviour
{
    [Header("Contents")]
    public bool containsKeys = false;

    [Header("Search Settings")]
    public float searchDuration = 1.5f;
    public bool disableColliderAfterFirstSearch = false;

    [Header("Flags")]
    public EventFlag hasKeysFlag;

    [Header("Result Text")]
    public string keysFoundText = "Pierce found the keys!";
    public string nothingFoundText = "Nothing useful here.";
    public string alreadyFoundText = "Pierce already has the keys.";

    [Header("Dialogue (optional)")]
    [Tooltip("If set, shows this dialogue when nothing useful is found instead of logging. Wait for dialogue to finish before ending search.")]
    public DialogueRunner dialogueRunner;
    [Tooltip("Dialogue to show when nothing found (e.g. one line: Pierce says 'Nothing useful here.').")]
    public DialogueAsset nothingFoundDialogue;
    [Tooltip("Optional. When player already has keys and searches again.")]
    public DialogueAsset alreadyFoundDialogue;

    [Header("Animation")]
    public PierceAnimationDriver animationDriver;

    [Header("Events")]
    public UnityEvent OnSearchStarted;
    public UnityEvent OnKeysFound;
    public UnityEvent OnSearchEmpty;
    public UnityEvent OnSearchComplete;

    private bool hasBeenSearched = false;
    private DialogueUI _dialogueUI;

    /// <summary>True while any SearchableProp search coroutine is running. Used to block other input.</summary>
    public static bool IsSearchInProgress { get; private set; }

    private void Start()
    {
        if (dialogueRunner == null)
            dialogueRunner = FindFirstObjectByType<DialogueRunner>();
        if (dialogueRunner != null)
            _dialogueUI = dialogueRunner.GetComponent<DialogueUI>();
    }

    public void Search()
    {
        if (hasBeenSearched)
        {
            Interactable.EndCurrentInteraction();
            return;
        }
        StartCoroutine(SearchCoroutine());
    }

    IEnumerator SearchCoroutine()
    {
        IsSearchInProgress = true;

        if (animationDriver != null)
            animationDriver.PlayInspectAndHold();

        OnSearchStarted?.Invoke();

        SearchPromptUI promptUI = SearchPromptUI.Instance != null ? SearchPromptUI.Instance : FindFirstObjectByType<SearchPromptUI>();
        if (promptUI != null)
            promptUI.Show("Searching...");

        yield return new WaitForSeconds(searchDuration);

        if (promptUI != null)
            promptUI.Hide();

        bool keysAlreadyFound = hasKeysFlag != null && hasKeysFlag.isActive;

        if (keysAlreadyFound)
        {
            if (_dialogueUI != null && alreadyFoundDialogue != null)
                yield return StartCoroutine(ShowDialogueAndWait(alreadyFoundDialogue));
            else
                Debug.Log("SEARCH: " + alreadyFoundText);
        }
        else if (containsKeys && !hasBeenSearched)
        {
            hasBeenSearched = true;

            if (hasKeysFlag != null)
                hasKeysFlag.Toggle();

            OnKeysFound?.Invoke();
        }
        else
        {
            if (_dialogueUI != null && nothingFoundDialogue != null)
                yield return StartCoroutine(ShowDialogueAndWait(nothingFoundDialogue));
            else
                Debug.Log("SEARCH: " + nothingFoundText);
            OnSearchEmpty?.Invoke();
        }

        hasBeenSearched = true;
        if (disableColliderAfterFirstSearch)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }
        if (animationDriver != null)
            animationDriver.ReleaseInspect();
        OnSearchComplete?.Invoke();

        IsSearchInProgress = false;
        Interactable.EndCurrentInteraction();
    }

    private IEnumerator ShowDialogueAndWait(DialogueAsset dialogue)
    {
        if (dialogue == null || _dialogueUI == null)
            yield break;

        bool finished = false;
        void OnFinished() { finished = true; }
        _dialogueUI.OnDialogueFinished += OnFinished;
        dialogueRunner.StartDialogue(dialogue);
        yield return new WaitUntil(() => finished);
        _dialogueUI.OnDialogueFinished -= OnFinished;
    }

    public void SetContainsKeys(bool value)
    {
        containsKeys = value;
    }

    public bool HasBeenSearched() => hasBeenSearched;
}