using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_Cursor : MonoBehaviour
{
    public bool HideCursor = true;
    private RectTransform rootCanvasRect;
    private Camera eventCamera;
    private RectTransform rectTrans;
    private Vector4 cursorData;
    private Vector2 CursorPos;
    private bool CursorHit;
    private Vector2 CursorOffset;
    void Start()
    {
        Canvas rootCanvas = GetComponentInParent<Canvas>();
        rootCanvasRect = rootCanvas.GetComponent<RectTransform>();
        eventCamera = rootCanvas.worldCamera;
        rectTrans = GetComponent<RectTransform>();
        cursorData = UI_MainCanvas.current.uIDisplay.material.GetVector("_CursorY");
        cursorData.y = rectTrans.rect.height / rootCanvasRect.sizeDelta.y;

        CursorOffset = rootCanvasRect.rect.size / 2;
    }
    void LateUpdate()
    {
        CursorHit = RectTransformUtility.ScreenPointToLocalPointInRectangle(rootCanvasRect, Mouse.current.position.ReadValue(), eventCamera, out CursorPos);
        if (CursorHit && rootCanvasRect.rect.Contains(CursorPos))
        {
            CursorPos += CursorOffset;
            rectTrans.anchoredPosition = CursorPos;
            UpdateShader();
        }
    }

    public void UpdateShader(float? CustomPosition = null)
    {
        if (CustomPosition != null)
        {
            cursorData.x = (float) CustomPosition;
        }
        else
        {
            cursorData.x = (CursorPos.y - rectTrans.rect.height / 4) / rootCanvasRect.rect.height;
        }
        UI_MainCanvas.current.uIDisplay.material.SetVector("_CursorY", cursorData);
    }
}
