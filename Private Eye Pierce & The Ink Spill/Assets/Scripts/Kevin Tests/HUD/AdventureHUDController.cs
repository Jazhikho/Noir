using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AdventureHUDController : MonoBehaviour
{
    [Header("UI Elements")]
    public Image cursorImage;
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

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (cursorImage != null)
            cursorRect = cursorImage.GetComponent<RectTransform>();

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

    void UpdateCursor()
    {
        if (cursorImage == null || cursorRect == null) return;

        cursorImage.enabled = true;

        Vector2 screenPos = Input.mousePosition;

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

        Vector2 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

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
