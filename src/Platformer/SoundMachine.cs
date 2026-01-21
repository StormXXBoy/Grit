using System;
using System.Collections.Generic;
using NAudio.Wave;

public class SoundMachine
{
    private Dictionary<string, AudioFileReader> audioFiles;
    private Dictionary<string, WaveOutEvent> outputs;

    public SoundMachine()
    {
        audioFiles = new Dictionary<string, AudioFileReader>();
        outputs = new Dictionary<string, WaveOutEvent>();
    }

    public void LoadSound(string key, string filePath)
    {
        if (audioFiles.ContainsKey(key))
        {
            audioFiles[key].Dispose();
            outputs[key].Dispose();
        }

        var reader = new AudioFileReader(filePath);
        var output = new WaveOutEvent();
        output.Init(reader);

        audioFiles[key] = reader;
        outputs[key] = output;
    }

    public void Play(string key, bool loop = false)
    {
        if (!audioFiles.ContainsKey(key)) return;

        if (loop)
        {
            audioFiles[key].Position = 0;
            outputs[key].PlaybackStopped += (s, e) =>
            {
                audioFiles[key].Position = 0;
                outputs[key].Play();
            };
        }

        Stop(key);
        outputs[key].Play();
    }

    public void Pause(string key)
    {
        if (outputs.ContainsKey(key))
            outputs[key].Pause();
    }

    public void Stop(string key)
    {
        if (outputs.ContainsKey(key))
        {
            outputs[key].Stop();
            audioFiles[key].Position = 0;
        }
    }
}
