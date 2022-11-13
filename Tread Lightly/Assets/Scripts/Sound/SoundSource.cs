public enum SoundSource
{
    Walking,
    Falling,
    Running,
}

public static class SoundSourceExtensions
{
    public static float GetFadeOff(this SoundSource soundSource)
    {
        return soundSource switch
        {
            SoundSource.Walking => 0.5f,
            SoundSource.Falling => 0.1f,
            SoundSource.Running => 0.25f,
            _ => 0f,
        };
    }

    public static float GetDefaultVolume(this SoundSource soundSource)
    {
        return soundSource switch
        {
            SoundSource.Walking => 1f,
            SoundSource.Falling => 4f,
            SoundSource.Running => 2f,
            _ => 0f,
        };
    }

    public static float GetFrequency(this SoundSource soundSource)
    {
        return soundSource switch
        {
            SoundSource.Walking => 0.5f,
            SoundSource.Falling => 0.25f,
            SoundSource.Running => 0.25f,
            _ => 0f,
        };
    }
}
