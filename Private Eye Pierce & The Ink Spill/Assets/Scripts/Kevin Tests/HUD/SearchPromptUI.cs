using UnityEngine;
using UnityEngine.UI;

public class SearchPromptUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelRoot;
    public Text searchingText;

    [Header("Text")]
    public string defaultText = "Searching...";

    public static SearchPromptUI Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    public void Show()
    {
        if (searchingText != null)
            searchingText.text = defaultText;

        if (panelRoot != null)
            panelRoot.SetActive(true);
    }

    public void Show(string text)
    {
        if (searchingText != null)
            searchingText.text = text;

        if (panelRoot != null)
            panelRoot.SetActive(true);
    }

    public void Hide()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    public void SetText(string text)
    {
        if (searchingText != null)
            searchingText.text = text;
    }
}
