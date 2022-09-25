namespace DoomSharp.Core.Sound;

internal class NullSoundDriver : ISoundDriver
{
    public void PauseSong(int handle)
    {
    }

    public void PlaySong(int handle, bool looping)
    {
    }

    public int RegisterSong(byte[] data)
    {
        return 1;
    }

    public void ResumeSong(int handle)
    {
    }

    public void SetChannels()
    {
    }

    public void SetMusicVolume(int v)
    {
    }

    public bool SoundIsPlaying(int handle)
    {
        return false;
    }

    public int StartSound(SoundType soundType, object? data, int volume, int sep, int pitch, int priority)
    {
        return 1;
    }

    public void StopSong(int handle)
    {
    }

    public void StopSound(int handle)
    {
    }

    public void UnregisterSong(int handle)
    {
    }
}