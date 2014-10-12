/// <summary>
/// Represents a type that provides audio frame data.
/// </summary>
public interface IAudioDataProvider
{
    event AudioDataReceivedEventHandler AudioDataReceived;
}

public delegate void AudioDataReceivedEventHandler(AudioFrameData frameData);