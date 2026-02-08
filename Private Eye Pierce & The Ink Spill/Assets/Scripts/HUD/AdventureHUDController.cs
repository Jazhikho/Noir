using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class AdventureHUDController : MonoBehaviour
{
    /// <summary>Cursor display mode: default, interactable (highlight), or door (arrow).</summary>
    public enum CursorType
    {
        Main,
        Interactable,
        Door
    }

    [Header("UI Elements")]
    public Image cursorImage;
    /// <summary>Sprite shown when cursor is over an interactable (e.g. bat, vending machine). Assign your highlight cursor image here.</summary>
    public Sprite interactableCursorSprite;
    /// <summary>Sprite shown when cursor is over a door. Assign your arrow/pointer image here.</summary>
    public Sprite doorCursorSprite;
    public Image highlightImage;
    public Image arrowImage;

    [Header("Detection")]
    public LayerMask interactableLayer;
    public float arrowDetectionRadius = 2f;

    [Header("Arrow Settings")]
    public Color arrowActiveColor = Color.white;
    public Color arrowLockedColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    [Header("Settings")]
    public bool hudEnabled = true;
    public bool hideSystemCursor = true;

    [Header("References")]
    public Transform playerTransform;
    public Camera mainCamera;

    public static AdventureHUDController Instance { get; private set; }

    private RectTransform cursorRect;
    private RectTransform highlightRect;
    private Canvas parentCanvas;
    private List<LockedDoor> lockedDoors = new List<LockedDoor>();
    private Sprite _mainCursorSprite;
    private CursorType _currentCursorType = CursorType.Main;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (cursorImage != null)
        {
            cursorRect = cursorImage.GetComponent<RectTransform>();
            _mainCursorSprite = cursorImage.sprite;
        }

        if (highlightImage != null)
            highlightRect = highlightImage.GetComponent<RectTransform>();

        parentCanvas = GetComponentInParent<Canvas>();

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Start()
    {
        if (playerTransform == null)
        {
            var player = FindFirstObjectByType<PointClickController>();
            if (player != null)
                playerTransform = player.transform;
        }

        lockedDoors.AddRange(FindObjectsByType<LockedDoor>(FindObjectsSortMode.None));

        SetHighlightVisible(false);
        SetArrowVisible(false);

        if (hideSystemCursor)
            Cursor.visible = false;

        EnsureCursorOnTop();
        if (cursorImage != null && hudEnabled && !cursorImage.gameObject.activeSelf)
            cursorImage.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!hudEnabled)
        {
            SetCursorVisible(false);
            SetHighlightVisible(false);
            SetArrowVisible(false);
            return;
        }

        UpdateCursor();
        UpdateHighlight();
        UpdateArrow();
    }

    /// <summary>
    /// Returns current mouse position in screen space. Uses new Input System when available so cursor stays in sync.
    /// </summary>
    private Vector2 GetMouseScreenPosition()
    {
        if (Mouse.current != null)
            return Mouse.current.position.ReadValue();
        return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }

    /// <summary>
    /// Puts the cursor as the last child of the canvas so it draws on top of other UI in the office scene.
    /// </summary>
    private void EnsureCursorOnTop()
    {
        if (cursorRect == null || parentCanvas == null) return;
        if (cursorRect.parent == parentCanvas.transform) return;
        cursorRect.SetParent(parentCanvas.transform, true);
        cursorRect.SetAsLastSibling();
    }

    void UpdateCursor()
    {
        if (cursorImage == null || cursorRect == null) return;

        if (!cursorImage.gameObject.activeSelf)
            cursorImage.gameObject.SetActive(true);
        cursorImage.enabled = true;

        if (cursorRect.parent != null)
            cursorRect.SetAsLastSibling();

        Vector2 screenPos = GetMouseScreenPosition();

        if (parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            cursorRect.position = screenPos;
        }
        else if (parentCanvas != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                screenPos,
                parentCanvas.worldCamera,
                out Vector2 localPoint
            );
            cursorRect.localPosition = localPoint;
        }
    }

    void UpdateHighlight()
    {
        if (highlightImage == null) return;

        Vector2 worldPos = mainCamera.ScreenToWorldPoint(GetMouseScreenPosition());

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, interactableLayer);

        if (hit.collider != null)
        {
            var highlightTarget = hit.collider.GetComponent<HoverHighlightTarget>();
            if (highlightTarget != null && highlightTarget.ShouldHighlight())
            {
                ShowHighlightAt(highlightTarget.GetHighlightPosition());
                return;
            }

            var interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                ShowHighlightAt(hit.collider.transform.position);
                return;
            }
        }

        SetHighlightVisible(false);
    }

    void ShowHighlightAt(Vector3 worldPos)
    {
        SetHighlightVisible(true);

        if (highlightRect != null && mainCamera != null)
        {
            Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPos);

            if (parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                highlightRect.position = screenPos;
            }
            else if (parentCanvas != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentCanvas.transform as RectTransform,
                    screenPos,
                    parentCanvas.worldCamera,
                    out Vector2 localPoint
                );
                highlightRect.localPosition = localPoint;
            }
        }
    }

    void UpdateArrow()
    {
        if (arrowImage == null || playerTransform == null) return;

        LockedDoor nearestDoor = null;
        float nearestDistance = float.MaxValue;

        foreach (var door in lockedDoors)
        {
            if (door == null || !door.showExitArrow) continue;
            if (!door.gameObject.activeInHierarchy) continue;

            float dist = Vector2.Distance(playerTransform.position, door.transform.position);
            if (dist < arrowDetectionRadius && dist < nearestDistance)
            {
                nearestDistance = dist;
                nearestDoor = door;
            }
        }

        if (nearestDoor != null)
        {
            SetArrowVisible(true);

            Vector2 screenPos = mainCamera.WorldToScreenPoint(nearestDoor.transform.position);
            if (arrowImage.rectTransform != null)
            {
                if (parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    arrowImage.rectTransform.position = screenPos;
            }

            arrowImage.color = nearestDoor.IsCurrentlyLocked() ? arrowLockedColor : arrowActiveColor;
        }
        else
        {
            SetArrowVisible(false);
        }
    }

    public void SetHUDEnabled(bool enabled)
    {
        hudEnabled = enabled;

        if (!enabled && hideSystemCursor)
            Cursor.visible = true;
        else if (enabled && hideSystemCursor)
            Cursor.visible = false;
    }

    public void SetCursorVisible(bool visible)
    {
        if (cursorImage != null)
            cursorImage.enabled = visible;
    }

    /// <summary>
    /// Sets the cursor appearance: Main (default), Interactable (highlight), or Door (arrow). Call from ClickController2D when hover target changes.
    /// </summary>
    public void SetCursorType(CursorType type)
    {
        if (_currentCursorType == type) return;
        _currentCursorType = type;

        if (cursorImage == null) return;
        Sprite s = GetSpriteForCursorType(type);
        if (s != null)
            cursorImage.sprite = s;
        else
            cursorImage.sprite = _mainCursorSprite;
    }

    private Sprite GetSpriteForCursorType(CursorType type)
    {
        if (type == CursorType.Interactable)
        {
            if (interactableCursorSprite != null) return interactableCursorSprite;
            if (highlightImage != null && highlightImage.sprite != null) return highlightImage.sprite;
        }
        else if (type == CursorType.Door)
        {
            if (doorCursorSprite != null) return doorCursorSprite;
            if (arrowImage != null && arrowImage.sprite != null) return arrowImage.sprite;
        }
        return null;
    }

    public void SetHighlightVisible(bool visible)
    {
        if (highlightImage != null)
            highlightImage.enabled = visible;
    }

    public void SetArrowVisible(bool visible)
    {
        if (arrowImage != null)
            arrowImage.enabled = visible;
    }

    public void RefreshLockedDoors()
    {
        lockedDoors.Clear();
        lockedDoors.AddRange(FindObjectsByType<LockedDoor>(FindObjectsSortMode.None));
    }

    void OnDestroy()
    {
        if (hideSystemCursor)
            Cursor.visible = true;
    }
}
