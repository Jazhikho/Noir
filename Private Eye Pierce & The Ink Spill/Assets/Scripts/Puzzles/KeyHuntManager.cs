using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class KeyHuntManager : MonoBehaviour
{
    [Header("Searchable Props")]
    public List<SearchableProp> searchableProps = new List<SearchableProp>();

    [Header("Flag")]
    public EventFlag hasKeysFlag;

    [Header("Randomization")]
    public bool randomizeKeyLocation = false;
    public SearchableProp fixedKeyProp;

    [Header("Events")]
    public UnityEvent OnKeysFound;

    public static KeyHuntManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (hasKeysFlag != null)
            hasKeysFlag.IsActiveChanged += OnFlagChanged;

        if (randomizeKeyLocation)
            RandomizeKeyLocation();
        else
            SetupFixedKeyLocation();

        foreach (var prop in searchableProps)
        {
            if (prop != null)
                prop.hasKeysFlag = hasKeysFlag;
        }
    }

    void OnDestroy()
    {
        if (hasKeysFlag != null)
            hasKeysFlag.IsActiveChanged -= OnFlagChanged;
    }

    void OnFlagChanged(bool value)
    {
        if (value)
            OnKeysFound?.Invoke();
    }

    public void RandomizeKeyLocation()
    {
        foreach (var prop in searchableProps)
        {
            if (prop != null)
                prop.SetContainsKeys(false);
        }

        if (searchableProps.Count > 0)
        {
            int randomIndex = Random.Range(0, searchableProps.Count);
            SearchableProp chosen = searchableProps[randomIndex];

            if (chosen != null)
            {
                chosen.SetContainsKeys(true);
                Debug.Log("KeyHuntManager: Keys randomly placed in " + chosen.gameObject.name);
            }
        }
    }

    void SetupFixedKeyLocation()
    {
        foreach (var prop in searchableProps)
        {
            if (prop != null)
                prop.SetContainsKeys(false);
        }

        if (fixedKeyProp != null)
        {
            fixedKeyProp.SetContainsKeys(true);
            Debug.Log("KeyHuntManager: Keys placed in " + fixedKeyProp.gameObject.name);
        }
    }

    public bool HasKeysBeenFound()
    {
        return hasKeysFlag != null && hasKeysFlag.isActive;
    }

    public int GetSearchedCount()
    {
        int count = 0;
        foreach (var prop in searchableProps)
        {
            if (prop != null && prop.HasBeenSearched())
                count++;
        }
        return count;
    }
}
