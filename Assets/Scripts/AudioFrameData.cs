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