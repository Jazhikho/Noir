using UnityEngine;

public class HoverHighlightTarget : MonoBehaviour
{
    [Header("Highlight Settings")]
    public bool highlightEnabled = true;
    public Transform highlightAnchor;
    public Vector2 highlightOffset = Vector2.zero;

    public Vector3 GetHighlightPosition()
    {
        Vector3 basePos;

        if (highlightAnchor != null)
            basePos = highlightAnchor.position;
        else
            basePos = transform.position;

        return basePos + (Vector3)highlightOffset;
    }

    public bool ShouldHighlight()
    {
        return highlightEnabled && gameObject.activeInHierarchy;
    }

    public void EnableHighlight()
    {
        highlightEnabled = true;
    }

    public void DisableHighlight()
    {
        highlightEnabled = false;
    }
}
