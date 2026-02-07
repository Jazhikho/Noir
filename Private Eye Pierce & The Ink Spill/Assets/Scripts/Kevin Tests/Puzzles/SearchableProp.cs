using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class SearchableProp : MonoBehaviour
{
    [Header("Contents")]
    public bool containsKeys = false;

    [Header("Search Settings")]
    public float searchDuration = 1.5f;

    [Header("Flags")]
    public EventFlag hasKeysFlag;

    [Header("Result Text")]
    public string keysFoundText = "Pierce found the keys!";
    public string nothingFoundText = "Nothing useful here.";
    public string alreadyFoundText = "Pierce already has the keys.";

    [Header("Animation")]
    public PierceAnimationDriver animationDriver;

    [Header("Events")]
    public UnityEvent OnSearchStarted;
    public UnityEvent OnKeysFound;
    public UnityEvent OnSearchEmpty;
    public UnityEvent OnSearchComplete;

    private bool hasBeenSearched = false;

    public void Search()
    {
        StartCoroutine(SearchCoroutine());
    }

    IEnumerator SearchCoroutine()
    {
        if (animationDriver != null)
            animationDriver.PlayInspect();

        OnSearchStarted?.Invoke();

        if (SearchPromptUI.Instance != null)
            SearchPromptUI.Instance.Show();

        Debug.Log("SEARCH: Searching " + gameObject.name + "...");

        yield return new WaitForSeconds(searchDuration);

        if (SearchPromptUI.Instance != null)
            SearchPromptUI.Instance.Hide();

        bool keysAlreadyFound = hasKeysFlag != null && hasKeysFlag.isActive;

        if (keysAlreadyFound)
        {
            Debug.Log("SEARCH: " + alreadyFoundText);
        }
        else if (containsKeys && !hasBeenSearched)
        {
            hasBeenSearched = true;

            if (hasKeysFlag != null)
                hasKeysFlag.Toggle();

            Debug.Log("SEARCH: " + keysFoundText);
            OnKeysFound?.Invoke();
        }
        else
        {
            Debug.Log("SEARCH: " + nothingFoundText);
            OnSearchEmpty?.Invoke();
        }

        hasBeenSearched = true;
        OnSearchComplete?.Invoke();

        Interactable.EndCurrentInteraction();
    }

    public void SetContainsKeys(bool value)
    {
        containsKeys = value;
    }

    public bool HasBeenSearched() => hasBeenSearched;
}
