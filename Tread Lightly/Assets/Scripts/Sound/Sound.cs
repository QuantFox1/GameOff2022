using System;
using UnityEngine;

public struct Sound
{
    public SoundSource Source { get; private set; }
    public float Volume { get; private set; } 
    public DateTime CreatedAt { get; private set; }
    public float CurrentVolume 
        => Volume * Mathf.Pow(Source.GetFadeOff(), (float)(DateTime.Now - CreatedAt).TotalMilliseconds / 1000f);
    public bool IsAudible
        => CurrentVolume > 0.1f;

    public Sound(SoundSource source, float volume)
    {
        Source = source;
        Volume = volume;
        CreatedAt = DateTime.Now;
    }
}
