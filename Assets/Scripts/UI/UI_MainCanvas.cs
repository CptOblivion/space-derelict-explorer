using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UI_MainCanvas : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool HideCursor = true;
    public static UI_MainCanvas current;
    public static RectTransform rectTrans;
    public UI_TargetSelector targetSelectorPrefab;
    public Camera mainCam;
    public Camera uICam;
    public RawImage uIDisplay;
    public Animator breachPreview;
    public RectTransform targetParent;
    public RectTransform previewParent;
    public RectTransform previewFrame;
    public RectTransform previewStem;
    public Camera wireframeCam;

    private UI_Cursor ui_Cursor;

    private static TextMeshProUGUI entryPreviewTitle;
    private void Awake()
    {
        current = this;
        entryPreviewTitle = previewFrame.GetComponentInChildren<TextMeshProUGUI>();
        rectTrans = GetComponent<RectTransform>();
        ui_Cursor = GetComponentInChildren<UI_Cursor>();
        ui_Cursor.gameObject.SetActive(false);

        previewParent.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui_Cursor.gameObject.SetActive(true);
        if (HideCursor)
        {
            POVCam.SetCursor(POVCam.CursorStates.none);
            Cursor.visible = false;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ui_Cursor.gameObject.SetActive(false);
        ui_Cursor.UpdateShader(2);
        if (HideCursor)
        {
            POVCam.SetCursor(POVCam.CursorStates.normal);
            Cursor.visible = true;
        }
    }

    public static void StartPreviewEntryPoint(string entryName)
    {
        entryPreviewTitle.text = entryName;
        current.previewParent.gameObject.SetActive(true);
    }

    public static void PreviewEntryPoint(Vector3 localPosition)
    {
        //TODO: make this not static (targets can call this function by finding this component in their parents)
        current.previewFrame.localPosition = localPosition;
        Vector2 pivot = new Vector2(-localPosition.x,-localPosition.y);
        pivot /= Mathf.Max(Mathf.Abs(pivot.x), Mathf.Abs(pivot.y));

        current.previewStem.localPosition = localPosition;
        current.previewStem.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(pivot.x, -pivot.y) * Mathf.Rad2Deg);

        pivot = new Vector2((pivot.x + 1) / 2, (pivot.y + 1) / 2);

        //if (localPosition.x < 0) pivot[0] = 1;
        //if (localPosition.y < 0) pivot[1] = 1;
        current.previewFrame.pivot = pivot;
    }

    public static void EndPreviewEntryPoint()
    {
        current.previewParent.gameObject.SetActive(false);
    }

    public void SetInteractable(bool interactable)
    {
        GetComponent<GraphicRaycaster>().enabled = interactable;
        //TODO: switch to LOD version when not in active use (update camera less frequently, or not at all?)
        //TODO: change station sun shadows to on-demand, only update those when we update the camera
            //use RequestShadowMapRendering() in the light's HDAdditionalLightData component
    }
}
