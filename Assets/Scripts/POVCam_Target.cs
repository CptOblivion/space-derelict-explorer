using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class POVCam_Target : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    //right click to back out to parent

    static POVCam_Target current = null;
    public bool Origin = false;
    public POVCam_Target parent;
    public UnityEvent OnMouseEnter;
    public UnityEvent OnMouseExit;
    public UnityEvent OnSelect; //do when clicked
    public UnityEvent OnArrive; //do when lerp/move path is done
    public UnityEvent OnLeave; //do when left
    public UnityEvent OnStart; //make sure children are in the right state, in case they were fiddled with in the inspector
    public float TravelTime = 2;

    static InputAction goBack = null;
    
    List<POVCam_Target> nodeChildren = new List<POVCam_Target>();
    Collider[] colliders;

    Camera targetCam;
    float lerping = 0;
    Camera lerpOrigin;

    void Start()
    {
        if (goBack == null)
        {
            goBack = POVCam.current.inputMap.FindAction("Back");
            goBack.performed += SelectNodeAction;
        }
        targetCam = GetComponentInChildren<Camera>(true);
        targetCam.gameObject.SetActive(false);
        if (parent)
        {
            parent.AddChild(this);
        }

        colliders = GetComponentsInChildren<Collider>();

        OnStart.Invoke();
        if (Origin && current == null) SelectNode();
        else gameObject.SetActive(false);
    }

    void Update()
    {
        if (lerping > 0)
        {
            lerping -= Time.deltaTime;
            if (lerping <= 0)
            {
                ArrivedAtNode();
            }
            float t = lerping / TravelTime;
            POVCam.cam.transform.position = Vector3.Lerp(targetCam.transform.position, lerpOrigin.transform.position, t);
            POVCam.cam.transform.rotation = Quaternion.Lerp(targetCam.transform.rotation, lerpOrigin.transform.rotation, t);
            POVCam.cam.fieldOfView = Mathf.Lerp(targetCam.fieldOfView, lerpOrigin.fieldOfView, t);
        }
    }

    public static void SelectNodeAction(InputAction.CallbackContext context)
    {
        if (current.parent) current.parent.SelectNode();
    }

    public void SelectNode()
    {
        if (current == this) return;
        if (current)
        {
            current.DeselectNode();
            OnSelect.Invoke();
            lerpOrigin = current.targetCam;
            lerping = TravelTime;
        }
        else
        {
            lerpOrigin = targetCam;
            lerping = 0.00001f;
        }
        gameObject.SetActive(true);
        EnableColliders(false);
        enabled = true;
        current = this;
        POVCam.SetCursor(POVCam.CursorStates.normal);
    }

    public void DeselectNode()
    {
        OnLeave.Invoke();
        foreach (POVCam_Target child in nodeChildren)
        {
            child.EnableColliders(false);
        }
        gameObject.SetActive(false);
    }

    void ArrivedAtNode()
    {
        lerping = 0;
        OnArrive.Invoke();
        foreach (POVCam_Target child in nodeChildren)
        {
            child.gameObject.SetActive(true);
            child.EnableColliders(true);
        }
    }

    public void EnableColliders(bool enabled)
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = enabled;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SelectNode();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseEnter.Invoke();
        POVCam.SetCursor(POVCam.CursorStates.hover);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseExit.Invoke();
        POVCam.SetCursor(POVCam.CursorStates.normal);
    }

    public void AddChild(POVCam_Target child)
    {
        nodeChildren.Add(child);
    }
}
