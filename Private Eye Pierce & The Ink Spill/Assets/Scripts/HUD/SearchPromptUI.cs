using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SearchPromptUI : MonoBehaviour
{
    [Header("UI References (Use TMP or Legacy Text)")]
    public GameObject panelRoot;
    public TMP_Text tmpText;
    public Text legacyText;

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
        SetText(defaultText);

        if (panelRoot != null)
            panelRoot.SetActive(true);
    }

    public void Show(string text)
    {
        SetText(text);

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
        if (tmpText != null)
            tmpText.text = text;
        else if (legacyText != null)
            legacyText.text = text;
    }
}