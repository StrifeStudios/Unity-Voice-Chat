using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Reconstructs audio data from an IAudioDataProvider into an audio clip.
/// </summary>
public class AudioDataReconstructor : MonoBehaviour
{
    private IAudioDataProvider dataSource;
    [SerializeField]
    private int recordingFrequency = 10000;
	private Queue<AudioFrameData> audioQueue = new Queue<AudioFrameData>();

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
        audioQueue.Enqueue(frameData);
        Play();
    }

	private bool isPlaying = false;
	private void Play()
	{
        if (!isPlaying)
        {
            isPlaying = true;
            StartCoroutine(PlayAudio());
        }
    }

	private IEnumerator PlayAudio()
	{
        while (audioQueue.Count > 0)
        {
            if (audio.isPlaying)
            {
                yield return new WaitForFixedUpdate();
            } 
            else
            {
                AudioFrameData frameData = audioQueue.Dequeue();
                audio.clip = AudioClip.Create("reconstructed", frameData.AudioData.Length, frameData.NumChannels, recordingFrequency, false, false);
                audio.clip.SetData(frameData.AudioData, 0);
                audio.Play();
            }
        }
        isPlaying = false;
    }
}
