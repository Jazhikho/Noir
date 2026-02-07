using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    //to add new cursor types, add a new name to the CursorName enum, declare a new public Texture2D and initialise in unity editor
    //then add a line to the Initiliase() method to add it to the cursor dictionary.
    public enum CursorName
    {
        main, look, use, pickup, navigateUp, carryItem, inventory
    }
    public Texture2D mainCursor;
    public Texture2D lookCursor;
    public Texture2D useCursor;
    public Texture2D pickupCursor;
    public Texture2D navigateCursorUp;
    public Texture2D carryItemCursor;
    public Texture2D inventoryCursor;
    private Dictionary<CursorName, Texture2D> cursors = new Dictionary<CursorName, Texture2D>();
    private Dictionary<CursorName, Vector2> cursorOffsets = new Dictionary<CursorName, Vector2>();
    private Dictionary<CursorName, Texture2D> readableCopies = new Dictionary<CursorName, Texture2D>();
    private bool carryingItem = false;
    private CursorName hiddenCursorChange = CursorName.main; //keeps record of cursor changes that are hidden while carrying an item so it can switch to correct cursor on item drop

    private void Awake()
    {
        Initialise();
    }

    public void Initialise()
    {
        if (cursors.Count > 0) return;

        cursors.Add(CursorName.main, mainCursor);
        cursors.Add(CursorName.look, lookCursor);
        cursors.Add(CursorName.use, useCursor);
        cursors.Add(CursorName.pickup, pickupCursor);
        cursors.Add(CursorName.navigateUp, navigateCursorUp);
        cursors.Add(CursorName.carryItem, carryItemCursor);
        cursors.Add(CursorName.inventory, inventoryCursor);

        cursorOffsets.Add(CursorName.main, Vector2.zero);
        cursorOffsets.Add(CursorName.look, Vector2.zero);
        cursorOffsets.Add(CursorName.use, Vector2.zero);
        cursorOffsets.Add(CursorName.pickup, Vector2.zero);
        cursorOffsets.Add(CursorName.navigateUp, new Vector2(16f, 0f));
        cursorOffsets.Add(CursorName.carryItem, Vector2.zero);
        cursorOffsets.Add(CursorName.inventory, new Vector2(0f, 0f));

        SafeSetCursor(CursorName.main);
    }

    /// <summary>
    /// Returns a texture safe for Cursor.SetCursor: always a runtime copy (RGBA32, readable, no mip) so imported textures never hit cursor API.
    /// </summary>
    private Texture2D GetReadableCursorTexture(CursorName name)
    {
        if (!cursors.TryGetValue(name, out Texture2D tex) || tex == null)
            return null;
        if (readableCopies.TryGetValue(name, out Texture2D copy) && copy != null)
            return copy;
        copy = CreateCursorSafeCopy(tex);
        if (copy != null)
            readableCopies[name] = copy;
        return copy;
    }

    /// <summary>
    /// Creates a copy that meets Cursor.SetCursor requirements: RGBA32, readable, no mip chain. Uses GPU blit so source can be any format.
    /// </summary>
    private static Texture2D CreateCursorSafeCopy(Texture2D source)
    {
        if (source == null || source.width <= 0 || source.height <= 0)
            return null;
        RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32);
        RenderTexture previous = RenderTexture.active;
        Graphics.Blit(source, rt);
        RenderTexture.active = rt;
        Texture2D copy = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        copy.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
        copy.Apply(false, false);
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);
        return copy;
    }

    /// <summary>
    /// Sets the cursor using a readable texture (original or GPU copy). Cursor.SetCursor requires CPU-accessible texture data.
    /// </summary>
    private void SafeSetCursor(CursorName name)
    {
        Texture2D tex = GetReadableCursorTexture(name);
        if (tex == null)
            return;
        Cursor.SetCursor(tex, cursorOffsets[name], CursorMode.Auto);
    }

    public void ChangeToCarryCursor(bool activate)
    {
        carryingItem = activate;
        if (activate)
        {
            SafeSetCursor(CursorName.carryItem);
            hiddenCursorChange = CursorName.main;
        }
        else
        {
            SafeSetCursor(hiddenCursorChange);
        }
    }

    public void ChangeCursor(CursorName c)
    {
        if (c == CursorName.carryItem)
        {
            Debug.Log("error, should call ChangeToCarryCursor directly");
            ChangeToCarryCursor(true);
            return;
        }

        if (!carryingItem) //only change cursor if not carrying an inventory item
        {
            SafeSetCursor(c);
        }
        else //but do store the change in case item is dropped
        {
            hiddenCursorChange = c;
        }
    }

}
