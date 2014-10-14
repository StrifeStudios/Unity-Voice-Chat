using UnityEngine;

/// <summary>
/// Reconstructs audio data from an IAudioDataProvider into an audio clip.
/// </summary>
public class AudioDataReconstructor : MonoBehaviour
{
    private IAudioDataProvider dataSource;
    [SerializeField]
    private int recordingFrequency = 10000;

    public IAudioDataProvider DataSource
    {
        get { return dataSource; }
        set
        {
            if (this.dataSource != null)
            {
                this.dataSource.AudioDataReceived -= OnAudioDataReceived;
            }
            if (value != null)
            {
                this.dataSource = value;
                this.dataSource.AudioDataReceived += OnAudioDataReceived;
            }
        }
    }

    public int RecordingFrequency
    {
        get { return recordingFrequency; }
        set { recordingFrequency = value; }
    }

    private void OnAudioDataReceived(AudioFrameData frameData)
    {
        // TODO: Make this dynamically fill an audio clip rather than create a new one every frame.
        audio.clip = AudioClip.Create("reconstructed", frameData.AudioData.Length, frameData.NumChannels, recordingFrequency, true, false);
        audio.clip.SetData(frameData.AudioData, 0);
        if (!audio.isPlaying)
            audio.Play();
    }
}
