using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class POVCam : MonoBehaviour
{
    public enum CursorStates { none, normal, hover};
    //this class doesn't really need to do anything but be findable

    public InputActionAsset inputMap;
    public static POVCam current;
    public static Camera cam;
    public Texture2D cursorNormal;
    public Texture2D cursorHover;
    void Awake()
    {
        current = this;
        cam = GetComponent<Camera>();
    }

    public static void SetCursor(CursorStates state)
    {
        switch (state)
        {
            case CursorStates.none:
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                break;
            case CursorStates.normal:
                Cursor.SetCursor(current.cursorNormal, Vector2.one * 32, CursorMode.Auto);
                break;
            case CursorStates.hover:
                Cursor.SetCursor(current.cursorHover, Vector2.one * 32, CursorMode.Auto);
                break;
        }
    }
}
