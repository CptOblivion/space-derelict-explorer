using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POVCam_Selectable : MonoBehaviour
{
    Material mat;
    public Material matOverride;

    private void Awake()
    {
        if (matOverride) mat = matOverride;
        else mat = GetComponent<MeshRenderer>().material;
    }
    public void Hover(bool enter)
    {
        mat.SetFloat("_glow", enter ? 1 : 0);
    }
}
