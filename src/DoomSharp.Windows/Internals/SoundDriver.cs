using System;
using System.IO;
using DoomSharp.Core.Sound;
using NAudio.Mixer;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace DoomSharp.Windows.Internals;

internal class SoundDriver : ISoundDriver, IDisposable
{
    // The number of internal mixing channels,
    //  the samples calculated for each mixing step,
    //  the size of the 16bit, 2 hardware channel (stereo)
    //  mixing buffer, and the samplerate of the raw data.


    // Needed for calling the actual sound output.
    public const int SAMPLECOUNT = 512;
    public const int NUM_CHANNELS = 8;
    // It is 2 for 16bit, and 2 for two channels.
    public const int BUFMUL = 4;
    public const int MIXBUFFERSIZE = (SAMPLECOUNT * BUFMUL);

    public const int SAMPLERATE = 11025;	// Hz
    public const int SAMPLESIZE = 2;   	// 16bit


    private readonly IWavePlayer _outputDevice;
    private readonly MixingSampleProvider _mixer;
    private readonly BufferedWaveProvider _bufferedWaveProvider;

    public SoundDriver()
    {
        _outputDevice = new WaveOutEvent();
        _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(SAMPLERATE, SAMPLESIZE))
        {
            ReadFully = true // plays "silence" if there's nothing else to play
        };

        _bufferedWaveProvider = new BufferedWaveProvider(WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, SAMPLERATE, SAMPLESIZE, SAMPLERATE, 8, 8))
        {
            ReadFully = true,
            DiscardOnBufferOverflow = true
        };

        AddMixerInput(_bufferedWaveProvider.ToSampleProvider());
        _outputDevice.Init(_mixer);
        _outputDevice.Play();
    }

    public void SetChannels() { }

    public int RegisterSong(byte[] data) => 1;
    public void PlaySong(int handle, bool looping) { }
    public void PauseSong(int handle) { }
    public void ResumeSong(int handle) { }
    public void StopSong(int handle) { }
    public void UnregisterSong(int handle) { }

    public void SetMusicVolume(int volume) { }

    public bool SoundIsPlaying(int handle) => false;

    public int StartSound(SoundType soundType, byte[] data, int volume, int stereoSeparation, int pitch, int priority)
    {
        // play these sound effects only one at a time
        if (soundType is SoundType.sfx_sawup or SoundType.sfx_sawidl or SoundType.sfx_sawful or SoundType.sfx_sawhit or SoundType.sfx_stnmov or SoundType.sfx_pistol)
        {
            // todo detect duplicate sound playing
        }

        using var ms = new MemoryStream(data, false);
        using var reader = new BinaryReader(ms);

        var format = reader.ReadUInt16();
        var sampleRate = reader.ReadUInt16();
        var sampleCount = reader.ReadUInt32() - 32; // padded with 16 bytes pre and post sample

        ms.Seek(16, SeekOrigin.Current);

        var sampleData = reader.ReadBytes((int)sampleCount);
        PlaySound(sampleData);

        return 1;
    }

    public void UpdateSoundParams(int handle, int volume, int stereoSeparation, int pitch) { }
    public void StopSound(int handle) { }

    public void SubmitSound() { }

    public void Dispose()
    {
        _outputDevice.Dispose();
    }

    private void PlaySound(byte[] data)
    {
        _bufferedWaveProvider.AddSamples(data, 0, data.Length);
    }

    private int GetQueuedAudioLength()
    {
        return _bufferedWaveProvider.BufferedBytes;
    }

    private void AddMixerInput(ISampleProvider input)
    {
        _mixer.AddMixerInput(ConvertToRightChannelCount(input));
    }

    private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
    {
        if (input.WaveFormat.Channels == _mixer.WaveFormat.Channels)
        {
            return input;
        }
        if (input.WaveFormat.Channels == 1 && _mixer.WaveFormat.Channels == 2)
        {
            return new MonoToStereoSampleProvider(input);
        }
        throw new NotImplementedException("Not yet implemented this channel count conversion");
    }
}
