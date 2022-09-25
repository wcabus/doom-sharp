namespace DoomSharp.Core.Sound;

public interface ISoundDriver
{
    void PauseSong(int handle);
    void PlaySong(int handle, bool looping);
    int RegisterSong(byte[] data);
    void ResumeSong(int handle);
    void SetChannels();
    void SetMusicVolume(int v);
    bool SoundIsPlaying(int handle);
    int StartSound(SoundType soundType, object? data, int volume, int sep, int pitch, int priority);
    void StopSong(int handle);
    void StopSound(int handle);
    void UnregisterSong(int handle);
}
