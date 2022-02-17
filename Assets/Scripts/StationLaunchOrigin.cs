using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationLaunchOrigin : MonoBehaviour
{
    public static StationLaunchOrigin current;
    public static LineRenderer pathRenderer;
    public int PathResolution = 12;
    public float PathWidthStart = .025f;
    public float PathWidthEnd = .015f;
    public Material pathMat;
    public Color pathSelected;
    public Color pathHighlight;
    private void Awake()
    {
        current = this;
        GameObject newOb = new GameObject("Path Renderer");
        newOb.layer = 5; //UI
        newOb.transform.SetParent(transform,false);
        pathRenderer = newOb.AddComponent<LineRenderer>();
        pathRenderer.positionCount = PathResolution;
        pathRenderer.material = pathMat;
        pathRenderer.enabled = false;
    }

    public static void NewPath()
    {
        current.pathMat.SetFloat("_TimeOffset", Time.timeSinceLevelLoad);
    }
    public static void DrawPath(Vector3 TargetPosition, Vector3 UpVec, float handleShift, bool selected)
    {
        //TODO: allow middle node to shift towards/away from origin (rather than just along upvec)
        pathRenderer.enabled = true;
        pathRenderer.startWidth = current.PathWidthStart;
        pathRenderer.endWidth = current.PathWidthEnd;
        if (selected)
            pathRenderer.startColor = pathRenderer.endColor = current.pathSelected;
        else
            pathRenderer.startColor = pathRenderer.endColor = current.pathHighlight;
        Vector3[] path = new Vector3[current.PathResolution];

        Vector3 Start = current.transform.position;
        Vector3 End = TargetPosition;
        Vector3 Handle = TargetPosition + UpVec;
        Vector3 ShiftVec = Vector3.Cross(Vector3.Cross(UpVec, Start - End), UpVec).normalized; //the ol' double-cross!
        Handle += ShiftVec * handleShift;
        Vector3 H1, H2;

        float pathstep;
        for (int i = 0; i < path.Length; i++)
        {
            pathstep = (float)i / (path.Length-1);
            H1 = Vector3.Lerp(Start, Handle, pathstep);
            H2 = Vector3.Lerp(Handle, End, pathstep);
            path[i] = Vector3.Lerp(H1, H2, pathstep);
        }
        pathRenderer.SetPositions(path);
    }

    public static void ClearPath()
    {
        pathRenderer.enabled = false;
    }
}
