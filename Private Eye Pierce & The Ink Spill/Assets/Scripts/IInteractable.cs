/// <summary>
/// Implemented by objects that can be clicked and hovered (e.g. doors, walk-to objects). Used by ClickController2D.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Called when the player clicks this interactable (e.g. start room transition or walk-to interaction).
    /// </summary>
    void OnClick();

    /// <summary>
    /// Called when the cursor enters or leaves this interactable. Use for cursor change or visual feedback.
    /// </summary>
    /// <param name="isHovering">True when cursor enters, false when it leaves.</param>
    void OnHover(bool isHovering);
}
