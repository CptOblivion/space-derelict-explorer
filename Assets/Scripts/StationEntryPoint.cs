using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationEntryPoint : MonoBehaviour
{
    static StationEntryPoint selectedEntryPoint = null;
    UI_TargetSelector selector;
    bool Previewing = false;
    bool Selected = false;
    public string Name = "Entry Point";
    public Transform entryPoint;
    public float HandleStrength = 5;
    public float HandleShift = 0;
    public Light flickerLight;
    float flickerLightIntensity;
    public Vector3 FlickerSpeeds = new Vector3(2, 6.28f, 15.3f);
    public Vector3 FlickerMagnitudes = new Vector3(1, .5f, .25f);


    public float Framerate = 3.5f;
    float frameTimer = 0;

    Camera previewCam;
    void Start()
    {
        previewCam = GetComponentInChildren<Camera>(true);
        selector = UI_TargetSelector.AddSelector(this);
        previewCam.gameObject.SetActive(false);
        previewCam.enabled = false;
        if (flickerLight)
        {
            flickerLight.enabled = false;
            flickerLightIntensity = flickerLight.intensity;
        }
        //eventually, store some data
    }

    private void Update()
    {
        if (Selected)
        {
            StationLaunchOrigin.DrawPath(entryPoint.position, entryPoint.up * HandleStrength, HandleShift, true);
            if (flickerLight)
            {
                flickerLight.intensity = flickerStrength() * flickerLightIntensity;
            }

            frameTimer -= Time.deltaTime;
            if (frameTimer <= 0)
            {
                UpdateCamera();
            }
        }
    }

    private void OnDestroy()
    {
        Destroy(selector);
    }

    public void PreviewEntry()
    {
        if (!Previewing)
        {
            Previewing = true;
            UI_MainCanvas.StartPreviewEntryPoint(Name);
        }
    }
    public void CancelPreviewEntry()
    {
        if (Previewing)
        {
            Previewing = false;
        }
    }

    float flickerStrength()
    {
        float flicker1 = Mathf.Sin(Time.timeSinceLevelLoad*FlickerSpeeds.x) * FlickerMagnitudes.x;
        float flicker2 = Mathf.Sin(Time.timeSinceLevelLoad * FlickerSpeeds.y) * FlickerMagnitudes.y;
        float flicker3 = Mathf.Sin(Time.timeSinceLevelLoad * FlickerSpeeds.z) * FlickerMagnitudes.z;
        return (flicker1 + flicker2 + flicker3)/FlickerMagnitudes.magnitude; 
    }

    public void SelectEntry()
    {
        CancelPreviewEntry();
        if (selectedEntryPoint)
        {
            selectedEntryPoint.DeselectEntry();
        }
        selectedEntryPoint = this;
        Selected = true;
        previewCam.gameObject.SetActive(true);
        UI_MainCanvas.current.breachPreview.ResetTrigger("Disable");
        UI_MainCanvas.current.breachPreview.SetTrigger("Enable");
        StationLaunchOrigin.NewPath();

        selector.setInteractable(false);

        if (flickerLight)
        {
            flickerLight.enabled = true;
        }

        UpdateCamera();

        //TODO: populate UI with details about entry (distance, travel time, temperature, whatever)
        //launch button for each unlaunched drone
    }
    public void DeselectEntry()
    {
        Selected = false;
        selector.setInteractable(true);
        StationLaunchOrigin.ClearPath();
        previewCam.gameObject.SetActive(false);
        UI_MainCanvas.current.breachPreview.SetTrigger("Disable");
        if (flickerLight)
        {
            flickerLight.enabled = false;
        }
    }

    public void Launch()
    {
        //play firing animation (follow bezier path)
        //when animation is done, load interior scene
    }
    void UpdateCamera()
    {
        //TODO: request shadow update for lights in preview
        previewCam.Render();

        frameTimer = 1f / Framerate;
    }
}
