using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a type that provides audio frame data.
/// </summary>
public interface IAudioDataProvider
{
    event AudioDataReceivedEventHandler AudioDataReceived;
}

public delegate void AudioDataReceivedEventHandler(AudioFrameData frameData);

/// <summary>
/// Immutable structure representing audio data for a single frame.
/// </summary>
public struct AudioFrameData
{
    public readonly float[] AudioData;
    public readonly int NumChannels;
    public AudioFrameData(float[] data, int numChannels)
    {
        this.AudioData = data;
        this.NumChannels = numChannels;
    }
}