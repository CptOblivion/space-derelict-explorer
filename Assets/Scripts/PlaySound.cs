using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [System.Serializable]
    public class AudioSourceExt
    {
        public string Name;
        public AudioSource source;
        [Range(0, 1)]
        public float Volume = 1;
        [Range(0, 1)]
        public float PitchRand = 0;
        [HideInInspector]
        public float basePitch = 1;
        public void Play()
        {
            source.pitch = basePitch + Random.Range(-PitchRand, PitchRand);
            source.Play();
        }
    }
    public AudioSourceExt[] sources;

    private void Awake()
    {
        foreach (AudioSourceExt sourceExt in sources)
        {
            sourceExt.basePitch = sourceExt.source.pitch;
        }
    }

    public void Play(int index)
    {
        sources[index].Play();
    }
}
