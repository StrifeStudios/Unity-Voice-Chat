/// <summary>
/// Collects microphone data via the Unity API and broadcasts that AudioClip data as it is becomes available.
/// </summary>
using UnityEngine;
public class UnityMicrophoneDataCollector : MonoBehaviour, IAudioDataProvider
{
    /// <summary>
    /// The AudioClip to monitor for recording and whose data will be broadcasted.
    /// </summary>
    public AudioClip RecordingClip
    {
        get { return recordingClip; }
        set
        {
            recordingClip = value;
            if (recordingClip == null)
            {
                this.enabled = false;
            }
            else
            {
                this.enabled = true;
            }

            lastSample = 0;
        }
    }

    private AudioClip recordingClip;
    private int lastSample = 0;
    private void Update()
    {
        int recordingPosition = Microphone.GetPosition(null);

        int diff = recordingPosition - lastSample;
        if (diff > 0)
        {
            float[] samples = new float[diff * recordingClip.channels];
            recordingClip.GetData(samples, lastSample);
            OnAudioDataReceived(samples, recordingClip.channels);
        }
        lastSample = recordingPosition;
    }

    // IAudioDataProvider implementation
    public event AudioDataReceivedEventHandler AudioDataReceived;
    public void OnAudioDataReceived(float[] data, int numChannels)
    {
        if (AudioDataReceived != null)
        {
            AudioFrameData frameData = new AudioFrameData(data, numChannels);
            AudioDataReceived(frameData);
        }
    }
}