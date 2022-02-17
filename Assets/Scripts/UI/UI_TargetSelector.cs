using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TargetSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public StationEntryPoint target;
    public Button button;
    bool Hovered = false;
    public UnityEvent OnHover;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        AlignOverTarget();
    }

    void Update()
    {
        AlignOverTarget();
        if (Hovered)
        {
            UI_MainCanvas.PreviewEntryPoint(transform.localPosition);
        }
        //check if target is visible (or on far side of station)
        //if on close side, position right on screen space position of target
        //if on far side, position around the edge of the station near as we can? or maybe just project through?
        //Or just ensure in design that all entry points are on the front at all times
    }


    void AlignOverTarget()
    {
        //TODO: factor in difference between rendertexture res and canvas size
        transform.localPosition = UI_MainCanvas.current.wireframeCam.WorldToScreenPoint(target.transform.position) - (Vector3) UI_MainCanvas.rectTrans.rect.size / 2;
    }
    public static Vector3 WorldToScreenSpace(Vector3 worldPos, Camera cam, RectTransform area)
    {
        Vector3 screenPoint = cam.WorldToScreenPoint(worldPos);
        screenPoint.z = 0;

        Vector2 screenPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(area, screenPoint, cam, out screenPos))
        {
            return screenPos;
        }

        return screenPoint;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            target.PreviewEntry();
            Hovered = true;
            OnHover.Invoke();
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (button.interactable)
        {
            target.CancelPreviewEntry();
            UI_MainCanvas.EndPreviewEntryPoint();
            Hovered = false;
        }
    }

    public void OnClick()
    {
        OnPointerExit(null);
        target.SelectEntry();
    }

    public void setInteractable(bool interactable)
    {
        button.interactable = interactable;
        image.raycastTarget = interactable;
    }

    public static UI_TargetSelector AddSelector(StationEntryPoint targetOb)
    {
        UI_TargetSelector newInstance = Instantiate(UI_MainCanvas.current.targetSelectorPrefab, UI_MainCanvas.current.targetParent.transform);
        newInstance.target = targetOb;
        return newInstance;
    }
}
